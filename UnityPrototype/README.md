# Unity Prototype (Playable MVP)

这是网页战斗迁移到 Unity 的 **可游玩 MVP 原型**。

## 已实现

- 网格数据：边界、邻接、曼哈顿距离（`GridBoard`）
- 单位模型：HP/SP/坐标/阵营（`UnitModel`）
- 回合循环：玩家回合 <-> 敌方回合，步数预算（`TurnController`）
- 玩家输入：`WASD` 移动、`Space` 攻击（`PlayerInputController`）
- 敌方 AI：向玩家逼近并在邻接时攻击（`EnemyAIController`）
- 可选 HUD：回合/步数/双方 HP SP/按钮（`BattleHudController`）
- 可选单位视图：模型位置跟随网格坐标（`UnitView`）

## 在 Unity 中如何游玩

1. 新建空场景，创建空物体 `BattleRoot`。
2. 给 `BattleRoot` 挂以下脚本：
   - `GridBoard`
   - `TurnController`
   - `EnemyAIController`
   - `PlayerInputController`
   - `BattleBootstrap`
3. （可选）创建 Canvas + Text + Button，并挂 `BattleHudController`。
4. （可选）创建两个单位 GameObject 挂 `UnitView`（玩家/敌人）。
5. 在 Inspector 绑定引用：
   - `EnemyAIController`：`TurnController`、`GridBoard`
   - `PlayerInputController`：`TurnController`、`GridBoard`
   - `BattleBootstrap`：`GridBoard`、`TurnController`、`EnemyAIController`、`PlayerInputController`、`BattleHudController`（可空）、`UnitView`（可空）
   - `BattleHudController`：`TurnController`、按钮和文本组件
6. 点击 Play：
   - 玩家回合：`WASD` 移动、`Space` 邻接攻击
   - 点击 End Turn（或 `BattleBootstrap > Player End Turn`）结束玩家回合
   - 敌方将自动行动

## 还没做完（下一阶段）

- 点击格子移动/攻击与可视高亮
- 技能卡、状态系统（眩晕/流血/怨念等）
- Boss 多阶段机制和演出
- 音效、剧情弹窗、完整 UI/日志面板
