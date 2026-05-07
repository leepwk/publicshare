function ensureActualHandshakesCheckboxList() {
  const existing = document.getElementById("actualHandshakes");
  if (!existing) return null;
  if (existing.classList.contains("checkbox-list")) return existing;

  const list = document.createElement("div");
  list.id = "actualHandshakes";
  list.className = "checkbox-list";
  list.setAttribute("role", "group");
  list.setAttribute("aria-label", "Hollywood handshakes");
  existing.replaceWith(list);
  return list;
}

function selectedActualHandshakeIds() {
  const list = ensureActualHandshakesCheckboxList();
  if (!list) return [];
  return Array.from(list.querySelectorAll('input[type="checkbox"]:checked')).map((input) => input.value).filter(Boolean);
}

function setSelectedActualHandshakeIds(ids) {
  const selected = new Set(ids || []);
  const list = ensureActualHandshakesCheckboxList();
  if (!list) return;
  list.querySelectorAll('input[type="checkbox"]').forEach((input) => {
    input.checked = selected.has(input.value);
  });
}

function renderActualHandshakeCheckboxes(bakers) {
  const list = ensureActualHandshakesCheckboxList();
  if (!list) return;
  const selected = new Set(selectedActualHandshakeIds());
  list.innerHTML = "";

  if (!bakers.length) {
    list.innerHTML = '<p class="muted">No bakers available.</p>';
    return;
  }

  for (const baker of bakers) {
    const id = `actualHandshake-${baker.id}`;
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

const originalFillBakerSelectForCheckboxes = fillBakerSelect;
fillBakerSelect = function (select, bakers, options = {}) {
  if (select?.id === "actualHandshakes") {
    renderActualHandshakeCheckboxes(bakers || []);
    return;
  }
  return originalFillBakerSelectForCheckboxes(select, bakers, options);
};

saveResults = async function (event) {
  event.preventDefault();
  if (!isAdmin()) return setText("resultStatus", "Admin access required.", true);
  setText("resultStatus", "Saving...");
  try {
    const weekId = $("resultWeekSelect").value;
    const eliminatedId = $("actualEliminated").value || null;
    const payload = {
      week_id: weekId,
      technical_winner_baker_id: $("actualTechnical").value || null,
      star_baker_id: $("actualStarBaker").value || null,
      eliminated_baker_id: eliminatedId,
      updated_at: new Date().toISOString(),
    };
    const resultRes = await state.supabase.from("results").upsert(payload, { onConflict: "week_id" }).select("id").single();
    if (resultRes.error) throw resultRes.error;
    const resultId = resultRes.data.id;
    const delHandshakeRes = await state.supabase.from("result_handshakes").delete().eq("result_id", resultId);
    if (delHandshakeRes.error) throw delHandshakeRes.error;
    const selectedHandshakeIds = selectedActualHandshakeIds();
    if (selectedHandshakeIds.length) {
      const insertRes = await state.supabase.from("result_handshakes").insert(selectedHandshakeIds.map((baker_id) => ({ result_id: resultId, baker_id })));
      if (insertRes.error) throw insertRes.error;
    }
    if (eliminatedId) {
      const elimRes = await state.supabase.from("bakers").update({ is_active: false, eliminated_week_id: weekId }).eq("id", eliminatedId);
      if (elimRes.error) throw elimRes.error;
    }
    setText("resultStatus", "Results saved. Active baker list updated.");
    await requireData();
    await loadResultForWeek();
    await renderLeaderboard();
  } catch (err) {
    setText("resultStatus", err.message || "Could not save results.", true);
  }
};

loadResultForWeek = async function () {
  if (!isAdmin()) return;
  try {
    const weekId = $("resultWeekSelect").value;
    const resultRes = await state.supabase.from("results").select("id, technical_winner_baker_id, star_baker_id, eliminated_baker_id").eq("week_id", weekId).maybeSingle();
    if (resultRes.error) throw resultRes.error;
    $("actualTechnical").value = resultRes.data?.technical_winner_baker_id || "";
    $("actualStarBaker").value = resultRes.data?.star_baker_id || "";
    $("actualEliminated").value = resultRes.data?.eliminated_baker_id || "";
    setSelectedActualHandshakeIds([]);
    if (resultRes.data?.id) {
      const hsRes = await state.supabase.from("result_handshakes").select("baker_id").eq("result_id", resultRes.data.id);
      if (hsRes.error) throw hsRes.error;
      setSelectedActualHandshakeIds((hsRes.data || []).map((x) => x.baker_id));
    }
  } catch (err) {
    setText("resultStatus", err.message || "Could not load result.", true);
  }
};

(function addCheckboxListStyles() {
  const style = document.createElement("style");
  style.textContent = `
    .checkbox-list {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 10px;
      width: 100%;
    }

    .checkbox-option {
      display: flex;
      align-items: center;
      gap: 10px;
      min-height: 44px;
      padding: 10px 12px;
      border: 1px solid var(--border);
      border-radius: 14px;
      background: #fffdfb;
      color: var(--ink);
      cursor: pointer;
    }

    .checkbox-option input {
      width: auto;
      accent-color: var(--brand);
    }

    @media (max-width: 760px) {
      .checkbox-list {
        grid-template-columns: 1fr;
      }
    }
  `;
  document.head.appendChild(style);
})();

ensureActualHandshakesCheckboxList();

(function addWeekLocking() {
  function isLockedWeekId(weekId) {
    return Boolean(state.weeks.find((week) => week.id === weekId)?.is_locked);
  }

  function weekLabel(week) {
    const lockedSuffix = week.is_locked ? " (locked)" : "";
    return `Week ${week.week_number} - ${week.title}${lockedSuffix}`;
  }

  const originalFillWeekSelectForLocks = fillWeekSelect;
  fillWeekSelect = function (select, selectedWeekId = "") {
    if (!select) return;
    const includeLocked = select.id !== "weekSelect";
    const weeks = includeLocked ? state.weeks : state.weeks.filter((week) => !week.is_locked);

    select.innerHTML = "";
    for (const week of weeks) {
      select.appendChild(option(week.id, weekLabel(week), week.id === selectedWeekId));
    }

    if (select.id === "weekSelect" && !weeks.length) {
      select.appendChild(option("", "No open weeks", true));
    }
  };

  function ensureWeekLockControls() {
    if (!isAdmin() || $("weekLockForm")) return;
    const currentWeekCard = $("currentWeekForm")?.closest(".card");
    if (!currentWeekCard) return;

    const section = document.createElement("section");
    section.className = "week-lock-controls";
    section.innerHTML = `
      <h3>Lock or unlock picks</h3>
      <p class="muted">Locked weeks are hidden from the player picks dropdown and cannot be saved to.</p>
      <form id="weekLockForm" class="grid">
        <label>Week
          <select id="lockWeekSelect" required></select>
        </label>
        <button id="toggleWeekLockButton" type="submit">Lock week</button>
      </form>
      <p id="weekLockStatus" class="status"></p>
    `;
    currentWeekCard.appendChild(section);
    $("weekLockForm").addEventListener("submit", toggleWeekLock);
    $("lockWeekSelect").addEventListener("change", updateWeekLockButton);
  }

  function updateWeekLockButton() {
    const select = $("lockWeekSelect");
    const button = $("toggleWeekLockButton");
    if (!select || !button) return;
    const week = state.weeks.find((item) => item.id === select.value);
    button.textContent = week?.is_locked ? "Unlock week" : "Lock week";
  }

  async function toggleWeekLock(event) {
    event.preventDefault();
    if (!isAdmin()) return setText("weekLockStatus", "Admin access required.", true);
    const weekId = $("lockWeekSelect").value;
    const week = state.weeks.find((item) => item.id === weekId);
    if (!week) return setText("weekLockStatus", "Choose a week.", true);

    setText("weekLockStatus", week.is_locked ? "Unlocking..." : "Locking...");
    try {
      const res = await state.supabase.from("weeks").update({ is_locked: !week.is_locked }).eq("id", weekId);
      if (res.error) throw res.error;
      setText("weekLockStatus", week.is_locked ? "Week unlocked." : "Week locked.");
      await requireData();
    } catch (err) {
      setText("weekLockStatus", err.message || "Could not update week lock.", true);
    }
  }

  const originalRequireDataForLocks = requireData;
  requireData = async function () {
    const result = await originalRequireDataForLocks();
    ensureWeekLockControls();
    fillWeekSelect($("lockWeekSelect"), $("lockWeekSelect")?.value || currentWeekId());
    updateWeekLockButton();
    return result;
  };

  const originalSavePredictionForLocks = savePrediction;
  savePrediction = async function (event) {
    const weekId = $("weekSelect")?.value;
    if (!weekId) {
      event.preventDefault();
      return setText("predictionStatus", "No open weeks are available for picks.", true);
    }
    if (isLockedWeekId(weekId)) {
      event.preventDefault();
      return setText("predictionStatus", "This week is locked, so picks can no longer be updated.", true);
    }
    return originalSavePredictionForLocks(event);
  };

  const originalLoadExistingPredictionForLocks = loadExistingPrediction;
  loadExistingPrediction = async function () {
    const weekId = $("weekSelect")?.value;
    if (!weekId) return setText("predictionStatus", "No open weeks are available for picks.", true);
    return originalLoadExistingPredictionForLocks();
  };
})();
