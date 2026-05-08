function initialiseAllPicksTab() {
  document.getElementById("refreshAllPicksButton")?.addEventListener("click", () => renderAllPicks?.());
}

const originalSwitchTabForAllPicks = switchTab;
switchTab = function (tabName) {
  const result = originalSwitchTabForAllPicks(tabName);
  if (tabName === "allPicks") renderAllPicks?.();
  return result;
};

document.addEventListener("DOMContentLoaded", initialiseAllPicksTab);
