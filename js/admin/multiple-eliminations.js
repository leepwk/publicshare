(function () {
  function selectedActualEliminationIds() {
    return Array.from(document.querySelectorAll('#actualEliminations input[type="checkbox"]:checked'))
      .map((input) => input.value)
      .filter(Boolean);
  }

  function setSelectedActualEliminationIds(ids) {
    const selected = new Set(ids || []);
    document.querySelectorAll('#actualEliminations input[type="checkbox"]').forEach((input) => {
      input.checked = selected.has(input.value);
    });
  }

  function renderActualEliminationCheckboxes(bakers) {
    const list = document.getElementById("actualEliminations");
    if (!list) return;

    const selected = new Set(selectedActualEliminationIds());
    list.innerHTML = "";

    if (!bakers.length) {
      list.innerHTML = '<p class="muted">No bakers available.</p>';
      return;
    }

    for (const baker of bakers) {
      const id = `actualElimination-${baker.id}`;
      const label = document.createElement("label");
      label.className = "checkbox-option";
      label.setAttribute("for", id);

      const input = document.createElement("input");
      input.type = "checkbox";
      input.id = id;
      input.value = baker.id;
      input.checked = selected.has(baker.id);

      const text = document.createElement("span");
      text.textContent = baker.name;

      label.append(input, text);
      list.appendChild(label);
    }
  }

  bakeoffApi.replaceResultEliminations = async function (resultId, bakerIds) {
    const res = await state.supabase.rpc("replace_result_eliminations", {
      p_result_id: resultId,
      p_baker_ids: bakerIds || [],
    });
    if (res.error) throw res.error;
  };

  bakeoffApi.getResultEliminationIds = async function (resultId) {
    const res = await state.supabase
      .from("result_eliminations")
      .select("baker_id")
      .eq("result_id", resultId);
    if (res.error) throw res.error;
    return (res.data || []).map((item) => item.baker_id);
  };

  const originalRenderAdminForms = renderAdminForms;
  renderAdminForms = function () {
    originalRenderAdminForms();
    if (isAdmin()) renderActualEliminationCheckboxes(state.bakers);
  };

  saveResults = async function (event) {
    event.preventDefault();
    if (!isAdmin()) return setText("resultStatus", "Admin access required.", true);
    setText("resultStatus", "Saving...");

    try {
      const result = await bakeoffApi.saveResult({
        week_id: $("resultWeekSelect").value,
        technical_winner_baker_id: $("actualTechnical").value || null,
        star_baker_id: $("actualStarBaker").value || null,
        updated_at: new Date().toISOString(),
      });

      await bakeoffApi.replaceResultHandshakes(result.id, selectedActualHandshakeIds());
      await bakeoffApi.replaceResultEliminations(result.id, selectedActualEliminationIds());

      setText("resultStatus", "Results saved. Active baker list updated.");
      await refreshAppData();
      await loadResultForWeek();
      await renderLeaderboardPage();
    } catch (err) {
      setText("resultStatus", err.message || "Could not save results.", true);
    }
  };

  loadResultForWeek = async function () {
    if (!isAdmin()) return;
    try {
      const weekId = $("resultWeekSelect").value;
      const result = await bakeoffApi.getResult(weekId);
      $("actualTechnical").value = result?.technical_winner_baker_id || "";
      $("actualStarBaker").value = result?.star_baker_id || "";
      setSelectedActualHandshakeIds([]);
      setSelectedActualEliminationIds([]);

      if (result?.id) {
        const [handshakeIds, eliminationIds] = await Promise.all([
          bakeoffApi.getResultHandshakeIds(result.id),
          bakeoffApi.getResultEliminationIds(result.id),
        ]);
        setSelectedActualHandshakeIds(handshakeIds);
        setSelectedActualEliminationIds(eliminationIds);
      }
    } catch (err) {
      setText("resultStatus", err.message || "Could not load result.", true);
    }
  };

  renderActualResults = async function () {
    const el = document.getElementById("actualResults");
    if (!el) return;

    try {
      const resultsRes = await state.supabase
        .from("results")
        .select("id, week_id, technical_winner_baker_id, star_baker_id");
      if (resultsRes.error) throw resultsRes.error;

      const results = resultsRes.data || [];
      if (!results.length) {
        el.innerHTML = "";
        return;
      }

      const resultIds = results.map((result) => result.id);
      const [handshakesRes, eliminationsRes] = await Promise.all([
        state.supabase.from("result_handshakes").select("result_id, baker_id").in("result_id", resultIds),
        state.supabase.from("result_eliminations").select("result_id, baker_id").in("result_id", resultIds),
      ]);
      if (handshakesRes.error) throw handshakesRes.error;
      if (eliminationsRes.error) throw eliminationsRes.error;

      const bakerName = (id) => state.bakers.find((baker) => baker.id === id)?.name || "";
      const weekLabel = (id) => {
        const week = state.weeks.find((item) => item.id === id);
        return week ? `Week ${week.week_number} - ${week.title}` : "";
      };

      const namesByResult = (rows) => {
        const map = new Map();
        for (const row of rows || []) {
          const names = map.get(row.result_id) || [];
          const name = bakerName(row.baker_id);
          if (name) names.push(name);
          map.set(row.result_id, names);
        }
        for (const names of map.values()) names.sort((a, b) => a.localeCompare(b));
        return map;
      };

      const handshakesByResult = namesByResult(handshakesRes.data);
      const eliminationsByResult = namesByResult(eliminationsRes.data);

      const rows = results
        .filter((result) =>
          result.technical_winner_baker_id ||
          result.star_baker_id ||
          (eliminationsByResult.get(result.id) || []).length ||
          (handshakesByResult.get(result.id) || []).length
        )
        .sort((a, b) => {
          const weekA = state.weeks.find((week) => week.id === a.week_id)?.week_number || 0;
          const weekB = state.weeks.find((week) => week.id === b.week_id)?.week_number || 0;
          return weekA - weekB;
        });

      el.innerHTML = rows.length
        ? `<h3>Results</h3><table><thead><tr><th>Week</th><th>Technical</th><th>Star baker</th><th>Eliminated</th><th>Hollywood handshakes</th></tr></thead><tbody>${rows.map((result) => `<tr><td>${escapeHtml(weekLabel(result.week_id))}</td><td>${escapeHtml(bakerName(result.technical_winner_baker_id) || "Not set")}</td><td>${escapeHtml(bakerName(result.star_baker_id) || "Not set")}</td><td>${escapeHtml((eliminationsByResult.get(result.id) || []).join(", ") || "None")}</td><td>${escapeHtml((handshakesByResult.get(result.id) || []).join(", ") || "None")}</td></tr>`).join("")}</tbody></table>`
        : "";
    } catch (err) {
      el.innerHTML = `<p class="status error">${escapeHtml(err.message || "Could not load actual results.")}</p>`;
    }
  };

  renderScoreBreakdown = async function () {
    const leaderboardEl = document.getElementById("leaderboard");
    if (!leaderboardEl || !state?.supabase) return;

    let el = document.getElementById("scoreBreakdown");
    if (!el) {
      el = document.createElement("div");
      el.id = "scoreBreakdown";
      leaderboardEl.insertAdjacentElement("afterend", el);
    }

    try {
      const [predictionsRes, resultsRes] = await Promise.all([
        state.supabase
          .from("predictions")
          .select("player_id, week_id, technical_winner_baker_id, star_baker_id, eliminated_baker_id, handshake_baker_id, players(name)"),
        state.supabase
          .from("results")
          .select("id, week_id, technical_winner_baker_id, star_baker_id"),
      ]);
      if (predictionsRes.error) throw predictionsRes.error;
      if (resultsRes.error) throw resultsRes.error;

      const results = resultsRes.data || [];
      if (!results.length) {
        el.innerHTML = "";
        return;
      }

      const resultIds = results.map((result) => result.id);
      const [handshakesRes, eliminationsRes] = await Promise.all([
        state.supabase.from("result_handshakes").select("result_id, baker_id").in("result_id", resultIds),
        state.supabase.from("result_eliminations").select("result_id, baker_id").in("result_id", resultIds),
      ]);
      if (handshakesRes.error) throw handshakesRes.error;
      if (eliminationsRes.error) throw eliminationsRes.error;

      const resultsByWeek = new Map(results.map((result) => [result.week_id, result]));
      const idsByResult = (rows) => {
        const map = new Map();
        for (const row of rows || []) {
          const ids = map.get(row.result_id) || new Set();
          ids.add(row.baker_id);
          map.set(row.result_id, ids);
        }
        return map;
      };
      const handshakesByResult = idsByResult(handshakesRes.data);
      const eliminationsByResult = idsByResult(eliminationsRes.data);

      const totalsByPlayer = new Map();
      for (const prediction of predictionsRes.data || []) {
        const result = resultsByWeek.get(prediction.week_id);
        if (!result) continue;

        const playerName = prediction.players?.name || "Unknown player";
        const row = totalsByPlayer.get(prediction.player_id) || {
          playerName,
          technical: 0,
          star: 0,
          eliminated: 0,
          handshake: 0,
        };

        if (prediction.technical_winner_baker_id && prediction.technical_winner_baker_id === result.technical_winner_baker_id) row.technical += 2;
        if (prediction.star_baker_id && prediction.star_baker_id === result.star_baker_id) row.star += 2;
        if (prediction.eliminated_baker_id && (eliminationsByResult.get(result.id) || new Set()).has(prediction.eliminated_baker_id)) row.eliminated += 2;
        if (prediction.handshake_baker_id && (handshakesByResult.get(result.id) || new Set()).has(prediction.handshake_baker_id)) row.handshake += 5;

        totalsByPlayer.set(prediction.player_id, row);
      }

      const rows = Array.from(totalsByPlayer.values())
        .map((row) => ({ ...row, total: row.technical + row.star + row.eliminated + row.handshake }))
        .sort((a, b) => b.total - a.total || a.playerName.localeCompare(b.playerName));

      el.innerHTML = rows.length
        ? `<h3>Score breakdown</h3><table><thead><tr><th>Player</th><th>Technical</th><th>Star baker</th><th>Eliminated</th><th>Handshake</th><th>Total</th></tr></thead><tbody>${rows.map((row) => `<tr><td>${escapeHtml(row.playerName)}</td><td>${row.technical}</td><td>${row.star}</td><td>${row.eliminated}</td><td>${row.handshake}</td><td>${row.total}</td></tr>`).join("")}</tbody></table>`
        : "";
    } catch (err) {
      el.innerHTML = `<p class="status error">${escapeHtml(err.message || "Could not load score breakdown.")}</p>`;
    }
  };
})();
