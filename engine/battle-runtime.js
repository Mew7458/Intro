(function battleRuntimeScope(global) {
  function bindReturnButton() {
    const backButton = document.querySelector('[data-action="return-menu"]');
    if (!backButton) return;
    backButton.addEventListener('click', () => {
      window.location.href = '../index.html';
    });
  }

  function updateStatus(text) {
    const status = document.getElementById('battle-status');
    if (status) status.textContent = text || '';
  }

  function setBattleFrameSource(stage) {
    const frame = document.getElementById('battle-frame');
    if (!frame) throw new Error('battle-frame not found');

    const legacyPath = stage && stage.legacy && stage.legacy.html;
    if (!legacyPath) {
      throw new Error(`Stage ${stage && stage.id ? stage.id : 'unknown'} is missing legacy html mapping`);
    }

    frame.onload = () => updateStatus('');
    frame.onerror = () => {
      updateStatus('战斗页面加载失败，正在尝试直接进入。');
      window.location.href = `../${legacyPath}`;
    };

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
    updateStatus('正在载入关卡...');

    const loader = global.GWStageLoader;
    if (!loader || typeof loader.resolveStageId !== 'function' || typeof loader.loadStageConfig !== 'function') {
      throw new Error('GWStageLoader is unavailable');
    }

    const stageId = loader.resolveStageId();
    const stage = await loader.loadStageConfig(stageId);
    renderStageHead(stage);
    setBattleFrameSource(stage);
  }

  document.addEventListener('DOMContentLoaded', () => {
    bootstrap().catch((error) => {
      console.error(error);
      updateStatus('关卡加载失败，请返回菜单重试。');
    });
  });
}(window));
