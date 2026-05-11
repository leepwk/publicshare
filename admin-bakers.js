function selectedAdminBaker() {
  const select = document.getElementById("adminBakerSelect");
  return state.bakers.find((baker) => baker.id === select?.value) || null;
}

function bakerPhotoPath(bakerId, file) {
  const ext = file.name.split(".").pop() || "jpg";
  return `bakers/${bakerId}.${ext}`;
}

async function uploadBakerPhoto(baker, file) {
  const path = bakerPhotoPath(baker.id, file);

  if (baker.avatar_path && baker.avatar_path !== path) {
    await state.supabase.storage.from(PLAYER_PHOTO_BUCKET).remove([baker.avatar_path]);
  }

  const upload = await state.supabase.storage.from(PLAYER_PHOTO_BUCKET).upload(path, file, { upsert: true });
  if (upload.error) throw upload.error;

  await bakeoffApi.updateBaker(baker.id, { avatar_path: path });
  return path;
}

async function refreshBakerAdminData() {
  state.bakers = await bakeoffApi.getBakers();
  state.activeBakers = state.bakers.filter((b) => b.is_active);
  renderForms();
  fillAdminBakerEditor();
  renderBakersDirectory?.();
}

function renderAdminBakerAvatar(baker) {
  const preview = document.getElementById("adminBakerAvatarPreview");
  if (!preview) return;

  if (!baker) {
    preview.innerHTML = '<p class="muted">Choose a baker to see their current avatar.</p>';
    return;
  }

  preview.innerHTML = `
    <div class="admin-baker-avatar-card">
      ${bakerAvatarHtml(baker)}
      <span>${escapeHtml(baker.name)}</span>
    </div>
  `;
}

function fillAdminBakerEditor() {
  const select = document.getElementById("adminBakerSelect");
  const information = document.getElementById("adminBakerInformation");
  if (!select || !information) return;

  const previous = select.value;
  select.innerHTML = "";
  select.appendChild(option("", "Choose baker", !previous));
  for (const baker of state.bakers) select.appendChild(option(baker.id, baker.name, baker.id === previous));

  const baker = selectedAdminBaker();
  information.value = baker?.information || "";
  renderAdminBakerAvatar(baker);
}

async function addBakerWithDetails(event) {
  event.preventDefault();
  event.stopImmediatePropagation();

  if (!isAdmin()) return setText("bakerStatus", "Admin access required.", true);

  const name = normaliseName(document.getElementById("newBakerName")?.value);
  const information = document.getElementById("newBakerInformation")?.value.trim() || null;
  const file = document.getElementById("newBakerPhoto")?.files?.[0];

  if (!name) return setText("bakerStatus", "Enter a baker name.", true);

  try {
    setText("bakerStatus", "Adding baker...");
    const baker = await bakeoffApi.addBaker({ name, information });

    if (file) {
      const avatarPath = await uploadBakerPhoto(baker, file);
      baker.avatar_path = avatarPath;
    }

    document.getElementById("newBakerName").value = "";
    document.getElementById("newBakerInformation").value = "";
    document.getElementById("newBakerPhoto").value = "";

    setText("bakerStatus", "Baker added.");
    await refreshBakerAdminData();
  } catch (err) {
    setText("bakerStatus", err.message || "Could not add baker.", true);
  }
}

async function updateAdminBaker(event) {
  event.preventDefault();
  if (!isAdmin()) return setText("adminBakerStatus", "Admin access required.", true);

  const baker = selectedAdminBaker();
  const information = document.getElementById("adminBakerInformation")?.value.trim() || null;
  const file = document.getElementById("adminBakerPhoto")?.files?.[0];

  if (!baker) return setText("adminBakerStatus", "Choose a baker.", true);

  try {
    setText("adminBakerStatus", "Saving...");
    await bakeoffApi.updateBaker(baker.id, { information });

    if (file) {
      await uploadBakerPhoto(baker, file);
      document.getElementById("adminBakerPhoto").value = "";
    }

    setText("adminBakerStatus", "Baker updated.");
    await refreshBakerAdminData();
  } catch (err) {
    setText("adminBakerStatus", err.message || "Could not update baker.", true);
  }
}

function addAdminBakerSection() {
  const adminTab = document.getElementById("adminTab");
  const currentWeekSection = document.getElementById("currentWeekForm")?.closest("section");
  if (!adminTab || document.getElementById("adminBakerSection")) return;

  const section = document.createElement("section");
  section.id = "adminBakerSection";
  section.className = "card";
  section.innerHTML = `
    <h2>Manage bakers</h2>
    <p class="muted">Update a baker's information and replace their avatar. Bakers are not deleted here.</p>
    <form id="adminBakerForm" class="grid">
      <label>Baker
        <select id="adminBakerSelect" required></select>
      </label>
      <label class="span-two">Information
        <textarea id="adminBakerInformation" placeholder="Add a short baker description"></textarea>
      </label>
      <div class="span-two admin-baker-avatar-tools">
        <h3>Current avatar</h3>
        <div id="adminBakerAvatarPreview"></div>
      </div>
      <label class="span-two">Replace photo
        <input id="adminBakerPhoto" type="file" accept="image/*">
      </label>
      <button type="submit">Update baker</button>
    </form>
    <p id="adminBakerStatus" class="status"></p>
  `;

  if (currentWeekSection) {
    adminTab.insertBefore(section, currentWeekSection);
  } else {
    adminTab.appendChild(section);
  }

  document.getElementById("adminBakerSelect")?.addEventListener("change", fillAdminBakerEditor);
  document.getElementById("adminBakerForm")?.addEventListener("submit", updateAdminBaker);
}

function loadAdminBakerTools() {
  if (!isAdmin()) return;
  addAdminBakerSection();
  fillAdminBakerEditor();
}

(function addAdminBakerStyles() {
  const style = document.createElement("style");
  style.textContent = `
    textarea {
      width: 100%;
      border: 1px solid var(--border);
      background: #fffaf7;
      color: var(--ink);
      border-radius: 14px;
      padding: 12px 14px;
      outline: none;
      font: inherit;
      min-height: 120px;
      resize: vertical;
    }

    textarea:focus {
      border-color: var(--brand);
      box-shadow: 0 0 0 4px rgba(201, 130, 150, 0.16);
    }

    .admin-baker-avatar-tools {
      margin-top: 6px;
    }

    .admin-baker-avatar-card {
      display: inline-flex;
      align-items: center;
      gap: 12px;
      padding: 10px 12px;
      border: 1px solid var(--border);
      border-radius: 16px;
      background: #fffdf9;
      margin-bottom: 12px;
    }
  `;
  document.head.appendChild(style);
})();

document.addEventListener("DOMContentLoaded", () => {
  document.getElementById("bakerForm")?.addEventListener("submit", addBakerWithDetails, true);
  document.querySelector('[data-tab="admin"]')?.addEventListener("click", loadAdminBakerTools);
});
