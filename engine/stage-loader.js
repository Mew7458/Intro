const STAGE_DEFAULTS = {
  id: 'intro',
  name: 'Intro',
  subtitle: '基础战斗演练',
  legacy: {
    html: 'intro-battle.html',
  },
};

const STAGE_MANIFEST = {
  intro: 'intro.stage.json',
  firstHeresy: 'firstHeresy.stage.json',
  bloodTowerPlan: 'bloodTowerPlan.stage.json',
  sevenSeas: 'sevenSeas.stage.json',
  abandonedAnimals: 'abandonedAnimals.stage.json',
  fatigue: 'fatigue.stage.json',
  oldLove: 'oldLove.stage.json',
  zaiBattle: 'zaiBattle.stage.json',
};

function deepMerge(base, patch) {
  const output = { ...base };
  Object.entries(patch || {}).forEach(([key, value]) => {
    if (
      value
      && typeof value === 'object'
      && !Array.isArray(value)
      && output[key]
      && typeof output[key] === 'object'
      && !Array.isArray(output[key])
    ) {
      output[key] = deepMerge(output[key], value);
      return;
    }
    output[key] = value;
  });
  return output;
}

export function resolveStageId() {
  const params = new URLSearchParams(window.location.search);
  const raw = params.get('stageId') || STAGE_DEFAULTS.id;
  return STAGE_MANIFEST[raw] ? raw : STAGE_DEFAULTS.id;
}

export async function loadStageConfig(stageId) {
  const fileName = STAGE_MANIFEST[stageId] || STAGE_MANIFEST[STAGE_DEFAULTS.id];
  const url = `../content/stages/${fileName}`;
  const response = await fetch(url, { cache: 'no-store' });
  if (!response.ok) {
    throw new Error(`Failed to load stage config: ${url}`);
  }

  const data = await response.json();
  return deepMerge(STAGE_DEFAULTS, data);
}
