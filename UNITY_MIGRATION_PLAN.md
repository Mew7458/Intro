# Unity 迁移尝试（针对 `lirathe-boss-battle`）

这个文档给出一个“先可运行、再逐步贴近原作”的迁移方案，目标是把当前网页版本的 2D 回合制战斗迁移到 Unity。

## 1. 迁移目标

- **第一阶段（可玩）**：实现网格、回合、单位移动、普通攻击、胜负判定。
- **第二阶段（对齐）**：补齐状态系统（眩晕、怨念、爆裂等）和 Boss 机制。
- **第三阶段（打磨）**：镜头、演出、UI、音效节奏尽量贴近网页版本。

## 2. 目录建议（Unity）

```text
Assets/
  Scripts/
    Core/
      BattleState.cs
      TurnController.cs
      GridBoard.cs
    Units/
      UnitModel.cs
      UnitView.cs
      UnitFactory.cs
    Skills/
      SkillDefinition.cs
      SkillExecutor.cs
    AI/
      EnemyAIController.cs
      Pathfinding.cs
    UI/
      BattleHudController.cs
      LogPanelController.cs
      StoryDialogController.cs
  ScriptableObjects/
    Units/
    Skills/
  Prefabs/
    Cell.prefab
    Unit.prefab
```

## 3. 核心数据映射

建议把网页脚本中的关键参数先映射为 ScriptableObject 或可序列化配置：

- 地图尺寸：`ROWS` / `COLS`
- 回合相关：`MAX_STEPS`、`BASE_START_STEPS`
- 单位模型：HP/SP、状态层数、朝向、阵营
- Boss 状态：phase、转阶段标记、陷阱/弱点点位

这样可减少硬编码，便于后续数值对齐。

## 4. 最小可运行流程（MVP）

1. 创建网格（9x26）。
2. 放置玩家 Karma 与敌人 Lirathe。
3. 点击单位 -> 高亮可移动区域。
4. 执行移动消耗步数。
5. 邻接攻击并结算 HP/SP。
6. 步数归零后结束回合并切换敌方 AI。

## 5. AI 迁移建议

- 先做“最近目标 + 曼哈顿距离”策略。
- 再补 BFS 路径与“无法攻击时消步/惩罚”逻辑。
- 加一个超时 watchdog（防止 AI 卡死）。

## 6. UI 对齐建议

网页中的 UI 分块可直接对应 Unity Canvas：

- 左侧：战场（Grid + Unit）
- 右侧：队伍状态、当前选择、手牌、结束回合按钮
- 底部/侧边：战斗日志
- 模态：胜利弹窗、剧情对话

## 7. 风险与取舍

- **可一比一复刻的部分**：规则逻辑、数值、流程。
- **不易像素级一致的部分**：字体渲染、CSS 动画细节、网页布局微差。

建议优先保证“玩法一致 + 反馈节奏一致”，再微调视觉表现。

## 8. 验收清单

- [ ] 3 回合内可完整进行“玩家行动 -> 敌方行动 -> 回合切换”。
- [ ] 基础状态（眩晕/流血/怨念）能正确生效并在 UI 展示。
- [ ] Boss 在指定条件下可正常进入下一阶段。
- [ ] 战斗日志可追踪关键事件。
- [ ] 胜利弹窗和剧情弹窗可触发。

---

已落地 `UnityPrototype/Assets/Scripts` 的第一阶段可玩骨架；下一步应优先补齐玩家点击交互、技能/状态系统与 Boss 多阶段机制。
