window.weekLocks = {
  isLocked(weeks, weekId) {
    return Boolean((weeks || []).find((week) => week.id === weekId)?.is_locked);
  },

  openWeeks(weeks) {
    return (weeks || []).filter((week) => !week.is_locked);
  },

  label(week, { showLockedLabel = true } = {}) {
    const lockedSuffix = showLockedLabel && week.is_locked ? " (locked)" : "";
    return `Week ${week.week_number} - ${week.title}${lockedSuffix}`;
  },
};
