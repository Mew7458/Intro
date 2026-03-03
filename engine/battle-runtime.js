import { loadStageConfig, resolveStageId } from './stage-loader.js';

function bindReturnButton() {
  const backButton = document.querySelector('[data-action="return-menu"]');
  if (!backButton) return;
  backButton.addEventListener('click', () => {
    window.location.href = '../index.html';
  });
}

function setBattleFrameSource(stage) {
  const frame = document.getElementById('battle-frame');
  if (!frame) throw new Error('battle-frame not found');

  const legacyPath = stage?.legacy?.html;
  if (!legacyPath) {
    throw new Error(`Stage ${stage?.id || 'unknown'} is missing legacy html mapping`);
  }

  frame.src = `../${legacyPath}`;
}

function renderStageHead(stage) {
  document.title = `${stage.name} - GOD'S WILL Demo`;

  const nameEl = document.getElementById('stage-name');
  const subtitleEl = document.getElementById('stage-subtitle');
  if (nameEl) nameEl.textContent = stage.name;
  if (subtitleEl) subtitleEl.textContent = stage.subtitle || '';
}

async function bootstrap() {
  bindReturnButton();

  const stageId = resolveStageId();
  const stage = await loadStageConfig(stageId);
  renderStageHead(stage);
  setBattleFrameSource(stage);
}

bootstrap().catch((error) => {
  console.error(error);
  const status = document.getElementById('battle-status');
  if (status) {
    status.textContent = '关卡加载失败，请返回菜单重试。';
  }
});
