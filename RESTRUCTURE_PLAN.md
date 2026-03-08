# 结构盘点与整理方案（参考 Limbus Company / Arknights）

## 1) 当前项目结构盘点（现状）

当前仓库以「单目录堆叠 + 关卡分散文件」为主：

- 顶层混放大量 `.html/.js/.css/.png/.mp3` 文件。
- 关卡文件命名模式较统一（如 `intro-battle.*`、`heresy-battle.*`、`blood-tower-battle.*`），但仍分布在根目录。
- 特效资源已有二级目录（`Effects/*`），音效有 `SFX/`，角色动作帧有 `Sprites/adora/*`，说明已有“按资产类别分层”的雏形。
- 仍存在备份文件（如 `.bak/.bak2/.backup`）和实现记录文档散落在根目录，增加维护负担。

> 结论：项目已经具备“战斗玩法中心化”的内容，但尚未形成可扩展的模块边界。

---

## 2) 参考结构：Limbus Company 与 Arknights（抽象到可落地层）

以下不是复制其源码，而是提炼其常见工程组织思想：

### 2.1 Limbus Company 可借鉴点（战斗逻辑与数值组织）

- **战斗规则强数据驱动**：角色技能、状态、资源（如理智/异常）倾向于配置化，而非散落在多个场景脚本。
- **实体层清晰**：角色、敌人、技能、状态效果、被动等实体可被独立描述并复用。
- **战斗流程分阶段**：回合开始、选技、结算、状态更新、回合结束等阶段边界清楚。

**对你项目的启发**：把“每个 boss 脚本中重复的战斗公共逻辑”抽成 engine 层；把“每个关卡差异”放入 data 配置。

### 2.2 Arknights 可借鉴点（关卡与内容管线）

- **关卡与地图配置独立**：地图、敌人波次、掉落/目标等与渲染层分离。
- **内容分包思路明确**：角色、敌人、UI、音频、特效通常按功能域与资源类型组织，便于增量更新。
- **版本化内容管理**：活动关卡、主线关卡、教程关卡分域管理，减少互相污染。

**对你项目的启发**：按“玩法域 + 内容域”双轴拆目录；每个关卡保留最薄入口，内容主要靠配置组装。

---

## 3) 建议目标结构（先整理目录，再逐步重构）

建议采用“代码 / 数据 / 资源”三层结构：

```text
Intro/
  src/
    core/                     # 通用系统：回合、网格、UI状态机、音频控制
      battle-engine/
      screen-router/
      save-progress/
    features/
      battle/
        entities/             # player/enemy/status/passive
        actions/              # 技能执行器与效果计算
        ai/
      chapter-select/
      character-panel/
      tutorial/
      farpvp/
    adapters/                 # firebase、浏览器API、兼容层

  data/
    stages/
      intro.json
      first-heresy.json
      blood-tower.json
      ...
    units/
      players/*.json
      enemies/*.json
      bosses/*.json
    skills/
      common/*.json
      bosses/*.json
    status-effects/*.json
    audio-cues.json

  assets/
    images/
      portraits/
      backgrounds/
      ui/
    sprites/
      adora/
      karma/
      dario/
      enemies/
    audio/
      bgm/
      sfx/
      voice/
    effects/
      blood-bite/
      blood-slash/
      ...

  scenes/
    index.html                # 主入口
    battle.html               # 通用战斗容器页面（长期目标：合并）

  docs/
    design/
    battle-spec/
    changelogs/
```

---

## 4) 分阶段实施方案（低风险）

### Phase 0：仅整理，不改逻辑（1~2 天）

- 建立 `assets/`、`docs/` 目录。
- 将图片/音频迁移到统一资产目录；先通过路径映射或批量替换保证可运行。
- 备份文件（`.bak/.bak2/.backup`）移到 `archive/`。

**产出**：目录更干净，功能不变。

### Phase 1：抽公共战斗内核（3~5 天）

- 从各 `*-battle-script.js` 提取公共能力：
  - 地图网格初始化
  - 单位回合与行动点
  - 伤害/状态结算
  - AI 选技基础流程
- 每个关卡脚本只保留：
  - 关卡专属事件
  - Boss 特有机制

**产出**：新增关卡时，复制粘贴量显著下降。

### Phase 2：关卡配置化（5~7 天）

- 建立 `data/stages/*.json`，抽离：
  - 地图尺寸与障碍
  - 初始站位
  - 波次与触发条件
  - 关卡对白与音频 cue
- 引入 `stage-loader` 读取配置并驱动战斗。

**产出**：策划向修改可在配置完成，减少改脚本风险。

### Phase 3：角色/技能配置化（持续）

- 将玩家与敌人的属性、技能池、被动逐步数据化。
- 保留复杂 boss 机制脚本化（混合模式），避免一次性全重写。

**产出**：更接近 Limbus/Arknights 风格的内容生产流程。

---

## 5) 针对你当前仓库的优先级建议

1. **先做目录清理与资产归类（最高优先级）**：投入小、收益立竿见影。  
2. **统一战斗页面模板（次优先级）**：把多个 battle html 收敛到 1 个容器页 + 数据入口。  
3. **最后再做深度配置化**：先稳定玩法内核，再迁移数据。

---

## 6) 风险与规避

- **风险 A：资源路径迁移导致 404**  
  规避：写一次性路径检查脚本；迁移后跑全关卡入口 smoke test。

- **风险 B：抽公共逻辑时破坏 boss 特殊机制**  
  规避：先抽无争议能力（网格/回合/基础伤害）；boss 机制先保留在关卡层。

- **风险 C：一次性大重构难回滚**  
  规避：按 Phase 分小 PR，每阶段可独立上线。

---

## 7) 一句话方案

**用 Arknights 的“内容分域 + 配置驱动”组织方式，结合 Limbus 的“战斗实体与结算流程模块化”，先完成目录与资源归类，再抽公共战斗内核，最后逐步把关卡/角色/技能数据化。**
