function selectedAdminPlayer() {
  const select = document.getElementById("adminPlayerSelect");
  return state.players.find((player) => player.id === select?.value) || null;
}

function renderAdminPlayerAvatar(player) {
  const preview = document.getElementById("adminPlayerAvatarPreview");
  const deleteButton = document.getElementById("deleteAdminPlayerAvatarButton");
  if (!preview || !deleteButton) return;

  preview.innerHTML = "";
  deleteButton.classList.add("hidden");

  if (!player) {
    preview.innerHTML = '<p class="muted">Choose a player to see their current avatar.</p>';
    return;
  }

  if (!player.avatar_path) {
    preview.innerHTML = '<p class="muted">No avatar uploaded for this player.</p>';
    return;
  }

  preview.innerHTML = `
    <div class="admin-player-avatar-card">
      ${avatarHtml(player.avatar_path, player.name)}
      <span>${escapeHtml(player.name)}</span>
    </div>
  `;
  deleteButton.classList.remove("hidden");
}

function fillAdminPlayerEditor() {
  const select = document.getElementById("adminPlayerSelect");
  const nameInput = document.getElementById("adminPlayerName");
  const photoSelect = document.getElementById("photoPlayerSelect");
  if (!select || !nameInput) return;

  const previous = select.value;
  select.innerHTML = "";
  select.appendChild(option("", "Choose player", true));
  for (const player of state.players) select.appendChild(option(player.id, player.name, player.id === previous));

  const player = selectedAdminPlayer();
  nameInput.value = player?.name || "";
  if (photoSelect && player) photoSelect.value = player.id;
  renderAdminPlayerAvatar(player);
}

async function refreshAdminPlayerData() {
  state.players = await bakeoffApi.getPlayers();
  fillPlayerSelect(document.getElementById("photoPlayerSelect"));
  fillAdminPlayerEditor();
  if (typeof loadPlayerNameOptions === "function") await loadPlayerNameOptions();
}

async function updateAdminPlayerName(event) {
  event.preventDefault();
  if (!isAdmin()) return setText("adminPlayerStatus", "Admin access required.", true);

  const player = selectedAdminPlayer();
  const newName = normaliseName(document.getElementById("adminPlayerName")?.value);
  if (!player) return setText("adminPlayerStatus", "Choose a player.", true);
  if (!newName) return setText("adminPlayerStatus", "Enter a player name.", true);

  try {
    setText("adminPlayerStatus", "Saving...");
    const update = await state.supabase.from("players").update({ name: newName }).eq("id", player.id);
    if (update.error) throw update.error;
    setText("adminPlayerStatus", "Player name updated.");
    await refreshAdminPlayerData();
    await renderLeaderboard();
  } catch (err) {
    setText("adminPlayerStatus", err.message || "Could not update player.", true);
  }
}

async function uploadAdminPlayerPhoto(event) {
  event.preventDefault();
  if (!isAdmin()) return setText("adminPlayerStatus", "Admin access required.", true);

  const player = selectedAdminPlayer();
  const file = document.getElementById("adminPlayerPhoto")?.files?.[0];
  if (!player) return setText("adminPlayerStatus", "Choose a player.", true);
  if (!file) return setText("adminPlayerStatus", "Choose a photo.", true);

  try {
    setText("adminPlayerStatus", "Uploading...");
    const ext = file.name.split(".").pop();
    const path = `${player.id}.${ext}`;

    if (player.avatar_path && player.avatar_path !== path) {
      await state.supabase.storage.from(PLAYER_PHOTO_BUCKET).remove([player.avatar_path]);
    }

    const upload = await state.supabase.storage.from(PLAYER_PHOTO_BUCKET).upload(path, file, { upsert: true });
    if (upload.error) throw upload.error;
    await bakeoffApi.updatePlayerAvatar(player.id, path);
    document.getElementById("adminPlayerPhoto").value = "";
    setText("adminPlayerStatus", "Player photo updated.");
    await refreshAdminPlayerData();
    await renderLeaderboard();
  } catch (err) {
    setText("adminPlayerStatus", err.message || "Could not upload photo.", true);
  }
}

async function deleteAdminPlayerAvatar() {
  if (!isAdmin()) return setText("adminPlayerStatus", "Admin access required.", true);

  const player = selectedAdminPlayer();
  if (!player) return setText("adminPlayerStatus", "Choose a player.", true);
  if (!player.avatar_path) return setText("adminPlayerStatus", "This player does not have an avatar to delete.", true);

  const ok = window.confirm(`Delete ${player.name}'s avatar?`);
  if (!ok) return;

  try {
    setText("adminPlayerStatus", "Deleting avatar...");
    const remove = await state.supabase.storage.from(PLAYER_PHOTO_BUCKET).remove([player.avatar_path]);
    if (remove.error) throw remove.error;

    const update = await state.supabase.from("players").update({ avatar_path: null }).eq("id", player.id);
    if (update.error) throw update.error;

    setText("adminPlayerStatus", "Player avatar deleted.");
    await refreshAdminPlayerData();
    await renderLeaderboard();
  } catch (err) {
    setText("adminPlayerStatus", err.message || "Could not delete avatar.", true);
  }
}

async function deleteAdminPlayer() {
  if (!isAdmin()) return setText("adminPlayerStatus", "Admin access required.", true);

  const player = selectedAdminPlayer();
  if (!player) return setText("adminPlayerStatus", "Choose a player.", true);

  const ok = window.confirm(`Delete ${player.name}? This will also delete their picks because predictions are linked to players with cascade delete.`);
  if (!ok) return;

  try {
    setText("adminPlayerStatus", "Deleting...");
    if (player.avatar_path) {
      await state.supabase.storage.from(PLAYER_PHOTO_BUCKET).remove([player.avatar_path]);
    }
    const del = await state.supabase.from("players").delete().eq("id", player.id);
    if (del.error) throw del.error;
    setText("adminPlayerStatus", "Player deleted.");
    await refreshAdminPlayerData();
    await renderLeaderboard();
  } catch (err) {
    setText("adminPlayerStatus", err.message || "Could not delete player.", true);
  }
}

function loadAdminPlayerTools() {
  if (!isAdmin()) return;
  refreshAdminPlayerData().catch((err) => setText("adminPlayerStatus", err.message || "Could not load players.", true));
}

document.addEventListener("DOMContentLoaded", () => {
  document.getElementById("adminPlayerSelect")?.addEventListener("change", fillAdminPlayerEditor);
  document.getElementById("adminPlayerNameForm")?.addEventListener("submit", updateAdminPlayerName);
  document.getElementById("adminPlayerPhotoForm")?.addEventListener("submit", uploadAdminPlayerPhoto);
  document.getElementById("deleteAdminPlayerAvatarButton")?.addEventListener("click", deleteAdminPlayerAvatar);
  document.getElementById("deleteAdminPlayerButton")?.addEventListener("click", deleteAdminPlayer);
  document.querySelector('[data-tab="admin"]')?.addEventListener("click", loadAdminPlayerTools);
});
