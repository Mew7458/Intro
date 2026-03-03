using System.Collections;
using Intro.UnityPrototype.AI;
using Intro.UnityPrototype.Units;
using UnityEngine;

namespace Intro.UnityPrototype.Core
{
    public class BattleBootstrap : MonoBehaviour
    {
        [SerializeField] private TurnController turnController;
        [SerializeField] private EnemyAIController enemyAI;

        private UnitModel karma;
        private UnitModel lirathe;

        private void Start()
        {
            karma = new UnitModel("karma", "Karma", BattleSide.Player, new Vector2Int(4, 21), 200, 50);
            lirathe = new UnitModel("lirathe", "Lirathe", BattleSide.Enemy, new Vector2Int(4, 4), 700, 80);

            enemyAI.Setup(lirathe, karma);
            turnController.OnTurnChanged += HandleTurnChanged;
            turnController.StartBattle();

            Debug.Log("Battle started. Player turn.");
        }

        private void HandleTurnChanged()
        {
            if (!karma.IsAlive || !lirathe.IsAlive)
            {
                var winner = karma.IsAlive ? karma.DisplayName : lirathe.DisplayName;
                Debug.Log($"Battle finished. Winner: {winner}");
                return;
            }

            if (turnController.CurrentSide == BattleSide.Enemy)
            {
                StartCoroutine(RunEnemyTurn());
            }
            else
            {
                Debug.Log($"Round {turnController.RoundCount + 1}: Player turn. Steps={turnController.PlayerSteps}");
            }
        }

        private IEnumerator RunEnemyTurn()
        {
            yield return enemyAI.ExecuteTurn();

            if (!karma.IsAlive)
            {
                Debug.Log("Karma defeated.");
            }
        }

        // 临时按钮：给玩家测试回合流转。
        [ContextMenu("Player End Turn")]
        public void PlayerEndTurn()
        {
            if (turnController.CurrentSide != BattleSide.Player) return;
            turnController.EndTurn();
        }

        [ContextMenu("Player Basic Attack")]
        public void PlayerBasicAttack()
        {
            if (turnController.CurrentSide != BattleSide.Player) return;
            if (!turnController.TryConsumeStep(BattleSide.Player, 1)) return;

            var dist = Mathf.Abs(karma.GridPosition.x - lirathe.GridPosition.x)
                       + Mathf.Abs(karma.GridPosition.y - lirathe.GridPosition.y);
            if (dist != 1)
            {
                Debug.Log("Target not adjacent.");
                return;
            }

            lirathe.ApplyDamage(30, 10);
            Debug.Log("Player attacks Lirathe (-30 HP, -10 SP)");
        }
    }
}
