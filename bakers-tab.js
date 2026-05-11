function bakerPhotoUrl(avatarPath) {
  return publicPhotoUrl(avatarPath);
}

function bakerAvatarHtml(baker) {
  const url = bakerPhotoUrl(baker.avatar_path);
  if (!url) {
    const initial = escapeHtml((baker.name || "B").trim().charAt(0).toUpperCase() || "B");
    return `<div class="baker-avatar baker-avatar-placeholder" aria-hidden="true">${initial}</div>`;
  }

  return `<img class="baker-avatar" src="${escapeAttribute(url)}" alt="${escapeAttribute(baker.name || "Baker photo")}">`;
}

function renderBakersDirectory() {
  const el = $("bakersDirectory");
  if (!el) return;

  if (!state.bakers.length) {
    el.innerHTML = `<p class="muted">No bakers added yet.</p>`;
    return;
  }

  el.innerHTML = `
    <div class="baker-grid">
      ${state.bakers.map((baker) => `
        <article class="baker-card">
          ${bakerAvatarHtml(baker)}
          <div class="baker-card-body">
            <div class="baker-card-heading">
              <h3>${escapeHtml(baker.name)}</h3>
              <span class="baker-status ${baker.is_active ? "active" : "eliminated"}">${baker.is_active ? "Active" : "Eliminated"}</span>
            </div>
            <p class="muted">${escapeHtml(baker.information || "No information added yet.")}</p>
          </div>
        </article>
      `).join("")}
    </div>
  `;
}

const originalSwitchTabForBakers = switchTab;
switchTab = function (tabName) {
  const result = originalSwitchTabForBakers(tabName);
  if (tabName === "bakers") renderBakersDirectory();
  return result;
};

(function addBakersTabStyles() {
  const style = document.createElement("style");
  style.textContent = `
    .baker-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 16px;
    }

    .baker-card {
      display: flex;
      gap: 16px;
      align-items: flex-start;
      border: 1px solid var(--border);
      border-radius: 18px;
      padding: 16px;
      background: #fffdf9;
    }

    .baker-avatar {
      width: 88px;
      height: 88px;
      border-radius: 999px;
      object-fit: cover;
      flex: 0 0 auto;
    }

    .baker-avatar-placeholder {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      background: #f9eaf0;
      color: var(--brand-dark);
      font-size: 2rem;
      font-weight: 800;
    }

    .baker-card-body {
      min-width: 0;
    }

    .baker-card-heading {
      display: flex;
      align-items: center;
      gap: 10px;
      flex-wrap: wrap;
      margin-bottom: 8px;
    }

    .baker-card-heading h3 {
      margin: 0;
    }

    .baker-status {
      display: inline-flex;
      align-items: center;
      border-radius: 999px;
      padding: 3px 9px;
      font-size: 0.74rem;
      font-weight: 800;
      border: 1px solid var(--border);
    }

    .baker-status.active {
      color: var(--brand-dark);
      background: #fff1f5;
    }

    .baker-status.eliminated {
      color: var(--muted);
      background: #fffaf7;
    }

    @media (max-width: 560px) {
      .baker-card {
        display: block;
      }

      .baker-avatar {
        margin-bottom: 12px;
      }
    }
  `;
  document.head.appendChild(style);
})();
