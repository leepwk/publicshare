function comboNormaliseName(name) {
  return (name || "").trim().replace(/\s+/g, " ");
}

async function loadPlayerNameOptions() {
  const list = document.getElementById("playerNameOptions");
  const client = state?.supabase;
  if (!list || !client) return;

  const { data, error } = await client.from("players").select("name").order("name");
  if (error) return;

  const seen = new Set();
  list.innerHTML = "";
  for (const player of data || []) {
    const name = comboNormaliseName(player.name);
    const key = name.toLowerCase();
    if (!name || seen.has(key)) continue;
    seen.add(key);

    const option = document.createElement("option");
    option.value = name;
    list.appendChild(option);
  }
}

document.addEventListener("DOMContentLoaded", () => {
  loadPlayerNameOptions();
  document.getElementById("predictionForm")?.addEventListener("submit", () => {
    setTimeout(loadPlayerNameOptions, 1000);
  });
});
