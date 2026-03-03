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

const STAGE_CONFIG_FALLBACK = {
  intro: { id: 'intro', name: 'Intro', subtitle: '基础战斗演练', legacy: { html: 'intro-battle.html' } },
  firstHeresy: { id: 'firstHeresy', name: '初见赫雷西', subtitle: '走入异端者领域', legacy: { html: 'heresy-battle.html' } },
  bloodTowerPlan: { id: 'bloodTowerPlan', name: '血楼计划', subtitle: '穿越污染核心地带', legacy: { html: 'blood-tower-battle.html' } },
  sevenSeas: { id: 'sevenSeas', name: '七海', subtitle: '面对七海作战队', legacy: { html: '7seaboss-battle.html' } },
  abandonedAnimals: { id: 'abandonedAnimals', name: '被遗弃的动物', subtitle: '追猎维尔米拉', legacy: { html: 'velmira-boss-battle.html' } },
  fatigue: { id: 'fatigue', name: '疲惫的极限', subtitle: '对决卡提亚', legacy: { html: 'khathia-boss-battle.html' } },
  oldLove: { id: 'oldLove', name: '旧情未了', subtitle: '直面莉拉瑟', legacy: { html: 'lirathe-boss-battle.html' } },
  zaiBattle: { id: 'zaiBattle', name: '宰', subtitle: '终局审判', legacy: { html: 'Zai-Battle.html' } },
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

async function loadFromJson(stageId) {
  const fileName = STAGE_MANIFEST[stageId] || STAGE_MANIFEST[STAGE_DEFAULTS.id];
  const url = `../content/stages/${fileName}`;
  const response = await fetch(url, { cache: 'no-store' });
  if (!response.ok) {
    throw new Error(`Failed to load stage config: ${url}`);
  }
  return response.json();
}

export async function loadStageConfig(stageId) {
  const normalized = STAGE_MANIFEST[stageId] ? stageId : STAGE_DEFAULTS.id;

  try {
    const data = await loadFromJson(normalized);
    return deepMerge(STAGE_DEFAULTS, data);
  } catch (error) {
    console.warn('[stage-loader] JSON load failed, using fallback map.', error);
    const fallback = STAGE_CONFIG_FALLBACK[normalized] || STAGE_CONFIG_FALLBACK[STAGE_DEFAULTS.id];
    return deepMerge(STAGE_DEFAULTS, fallback);
  }
}
