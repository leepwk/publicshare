function makeLeaderboardFirst() {
  const tabs = document.querySelector(".tabs");
  const leaderboardButton = document.querySelector('[data-tab="leaderboard"]');
  const entryButton = document.querySelector('[data-tab="entry"]');
  const appView = document.getElementById("appView");
  const leaderboardTab = document.getElementById("leaderboardTab");
  const entryTab = document.getElementById("entryTab");

  if (tabs && leaderboardButton && entryButton) {
    tabs.insertBefore(leaderboardButton, entryButton);
  }

  if (appView && leaderboardTab && entryTab) {
    appView.insertBefore(leaderboardTab, entryTab);
  }
}

document.addEventListener("DOMContentLoaded", makeLeaderboardFirst);
