# 关卡结构梳理与统一方案（参考 Limbus Company / Arknights）

## 1) 当前项目结构速览

通过本地仓库扫描，当前战斗关卡是“每关一套 HTML/CSS/JS”的平铺结构：

- 关卡页面：`intro-battle.html`、`heresy-battle.html`、`blood-tower-battle.html`、`7seaboss-battle.html`、`Zai-Battle.html`、`velmira-boss-battle.html`、`khathia-boss-battle.html`、`lirathe-boss-battle.html`、`pvp-battle.html`、`farpvp-battle.html`
- 关卡逻辑脚本：上述页面基本都对应各自的 `*-script.js`
- 关卡样式：上述页面基本都对应各自的 `*-styles.css`
- 主菜单/关卡入口集中在 `index.html + script.js`

现状优点：
- 单关可快速独立迭代。
- 临时活动玩法可直接复制一份开做。

现状问题（也是你提到的核心）：
- 逻辑分叉严重：同类系统（回合、AI、技能结算、日志、胜败弹窗）在多个大脚本中重复实现。
- 修改成本高：修复一个底层 bug 往往要改多个 `*-script.js`。
- 表现层不一致：不同关卡 HTML 结构略有差异，导致 UI 功能开关不统一。
- 难以“按章节扩关”：每新增一关都要复制三件套（html/css/js），长期不可控。

---

## 2) 参考项目的结构思路（抽象到可落地层面）

> 这里不是还原对方源码，而是提炼“工程组织方式”。

### A. Limbus Company（可借鉴点）

Limbus 的体验特征是：
- 战斗主循环高度统一（回合推进、速度/行动顺序、技能对撞/结算、状态系统）。
- 关卡差异主要通过“配置数据 + 敌人/事件脚本”体现，而不是每关换一套前端壳。
- 剧情/战斗/演出是“同一套流程节点”下的不同阶段（切状态而不是切页面）。

可借鉴：
1. **统一战斗壳（Battle Runtime）**：所有关卡跑同一引擎。
2. **关卡数据驱动（Stage Config）**：地图、敌方编成、胜负条件、奖励都写配置。
3. **事件节点化（Stage Events）**：第 N 回合触发对话、BOSS 转阶段、增援入场等。

### B. Arknights（可借鉴点）

Arknights 的典型工程模式是：
- 统一战斗场景（单一战斗容器），关卡主要由地图与波次/出生点/时间轴配置描述。
- 明确“基础规则层”与“关卡特化层”分离：路径、地块、敌人波次是配置；特殊机关是可插拔逻辑。
- 主界面与战斗解耦：选关是前台 UI，进入后加载关卡包。

可借鉴：
1. **Map + Wave 的关卡 DSL**：地图与敌人刷新规则标准化。
2. **规则插件化**：例如“血楼机制”“雾天机制”作为 stage mutator/规则模块。
3. **统一入口页面**：`battle.html?stageId=...` 或路由参数加载不同关卡。

---

## 3) 建议的目标结构（适配你当前项目）

### 3.1 目录重构（建议）

```text
/engine
  battle-runtime.js        # 回合推进、行动队列、伤害结算、状态机
  grid-system.js           # 地图、寻路、掩体、距离计算
  ai-runtime.js            # 通用 AI 决策骨架
  ui-runtime.js            # 面板渲染、日志、弹窗、技能栏

/content
  /stages
    intro.stage.json
    heresy.stage.json
    bloodTower.stage.json
    sevenSeas.stage.json
  /units
    enemies.json
    players.json
  /skills
    skills.json
  /events
    common-events.js

/rules
  blood-tower-rule.js      # 特殊机制插件
  heresy-rule.js

/scenes
  battle.html              # 统一战斗页面

/app
  menu.js                  # 选关/角色页逻辑（原 script.js 逐步拆分）
```

### 3.2 核心原则

1. **页面单一化**：从多 `*-battle.html` 收敛到 1 个 `battle.html`。
2. **逻辑引擎化**：从多 `*-script.js` 收敛到 `engine/*`。
3. **内容数据化**：关卡内容写 `.stage.json`，而不是把参数硬编码在脚本顶部。
4. **机制插件化**：特殊规则（Boss phase、天气、地图机关）做成 `rules/*.js`。

---

## 4) 统一关卡配置草案（示例）

```json
{
  "id": "bloodTower",
  "name": "血楼计划",
  "map": {
    "rows": 13,
    "cols": 17,
    "voidCells": [],
    "coverCells": [[4,7], [4,8]]
  },
  "playerSpawn": [
    { "unitId": "adora", "r": 12, "c": 3 },
    { "unitId": "karma", "r": 12, "c": 4 }
  ],
  "enemyWaves": [
    {
      "trigger": { "type": "turn", "value": 1 },
      "units": [{ "unitId": "blood_guard", "r": 2, "c": 10 }]
    },
    {
      "trigger": { "type": "hpBelow", "target": "boss_blood", "value": 0.5 },
      "units": [{ "unitId": "blood_priest", "r": 1, "c": 9 }]
    }
  ],
  "victory": { "type": "kill", "target": "boss_blood" },
  "defeat": { "type": "allPlayerDead" },
  "rules": ["bloodTowerRule"],
  "rewards": { "coin": 3 }
}
```

---

## 5) 分阶段迁移路线（避免一次性重写）

### Phase 0（1~2 天）：建立“统一壳”但不改玩法
- 新建 `scenes/battle.html`，先复用现有通用 DOM 结构。
- 保留旧关卡文件可用，新增一条新入口用于试运行。

### Phase 1（3~5 天）：抽离共通引擎
- 把所有关卡中重复的模块先抽出：
  - 网格与寻路
  - 回合推进
  - 技能释放与日志
  - 胜败判定
- 形成 `engine/*`，关卡脚本改成仅“喂配置 + 小量钩子”。

### Phase 2（3~7 天）：数据化 2 个样板关卡
- 先迁移 `intro` + `heresy` 到 `.stage.json`。
- 验证：只改 JSON 是否能完成地图、敌人、奖励差异。

### Phase 3（5~10 天）：机制插件化
- 将“血楼计划”“七海”等特殊机制迁移到 `rules/*.js`。
- 引擎只提供钩子：`onTurnStart/onDamage/onUnitDead/onRoundEnd`。

### Phase 4（持续）：清理旧文件
- 每迁完一关，删除对应旧 `*-battle.html/js/css`。
- 最终路由统一为：
  - `index.html` 选关 -> `battle.html?stageId=intro`

---

## 6) 立即可执行的最小改造清单（本周）

1. 在 `script.js` 的“进入关卡”逻辑里，先支持新路由：
   - `battle.html?stageId=xxx`
2. 新建 `content/stages/intro.stage.json`。
3. 新建 `engine/stage-loader.js`（负责读取 stageId + 合并默认值）。
4. 新建 `engine/battle-runtime.js` 最小版（先接管回合与胜败判定）。
5. 保持旧关卡链接作为 fallback（保证可回滚）。

---

## 7) 风险与规避

- 风险 1：重构期功能倒退。
  - 规避：每迁移一关就做“旧版对照回归”（胜败、技能、UI 交互）。
- 风险 2：特化机制过多导致“伪通用”。
  - 规避：核心引擎只保留 80% 通用能力，20% 通过 rule 插件注入。
- 风险 3：数据配置膨胀难维护。
  - 规避：把单位/技能词条独立为字典，stage 只引用 ID。

---

## 8) 建议结论

你现在的方向判断是对的：
- **不要继续“每关一份 HTML/JS/CSS”**。
- 先做“统一战斗页面 + 关卡配置化 + 特殊机制插件化”。

如果你愿意，我下一步可以直接给出：
1) `battle.html` 的最小通用骨架；
2) `intro.stage.json` 的首版；
3) `stage-loader + runtime` 的可跑通骨架代码（可与现有脚本并行，不会一次性打爆现有内容）。
