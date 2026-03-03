# Unity Prototype (MVP Start)

这是一个可直接拷贝进 Unity 项目的**起步代码骨架**，用于把当前网页战斗逻辑逐步迁移到 Unity。

## 已提供脚本

- `Assets/Scripts/Units/UnitModel.cs`：单位数据模型（HP/SP/阵营/坐标）。
- `Assets/Scripts/Core/GridBoard.cs`：网格边界、可行走判定、邻接格获取。
- `Assets/Scripts/Core/TurnController.cs`：回合切换与步数预算。
- `Assets/Scripts/AI/EnemyAIController.cs`：基础敌方 AI（靠近并邻接攻击）。
- `Assets/Scripts/Core/BattleBootstrap.cs`：战斗启动与调试入口。

## 使用方式

1. 在 Unity 新建空场景。
2. 创建空物体 `BattleRoot`，挂载：
   - `GridBoard`
   - `TurnController`
   - `EnemyAIController`
   - `BattleBootstrap`
3. 在 `BattleBootstrap` 中拖拽绑定 `TurnController` 与 `EnemyAIController`。
4. 在 `EnemyAIController` 中拖拽绑定 `TurnController` 与 `GridBoard`。
5. 运行后通过 `BattleBootstrap` 的 ContextMenu（Inspector 右上角）测试：
   - `Player End Turn`
   - `Player Basic Attack`

## 现阶段说明

- 当前是 **MVP 逻辑骨架**，尚未包含完整 UI、技能卡、状态层数系统、Boss 多阶段演出。
- 目标是先跑通“玩家回合 -> 敌方回合 -> 回合循环”，再逐步接入现有网页规则细节。
