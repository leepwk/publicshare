function initialiseAllPicksTab() {
  document.getElementById("refreshAllPicksButton")?.addEventListener("click", () => renderAllPicksPage?.());
}

document.addEventListener("DOMContentLoaded", initialiseAllPicksTab);
