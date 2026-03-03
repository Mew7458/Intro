using System.Collections;
using Intro.UnityPrototype.Core;
using Intro.UnityPrototype.Units;
using UnityEngine;

namespace Intro.UnityPrototype.AI
{
    public class EnemyAIController : MonoBehaviour
    {
        [SerializeField] private TurnController turnController;
        [SerializeField] private GridBoard board;
        [SerializeField] private float thinkDelaySeconds = 0.5f;

        private UnitModel enemy;
        private UnitModel player;

        public void Setup(UnitModel enemyUnit, UnitModel playerUnit)
        {
            enemy = enemyUnit;
            player = playerUnit;
        }

        public IEnumerator ExecuteTurn()
        {
            if (enemy == null || player == null)
            {
                Debug.LogWarning("EnemyAIController 未初始化 Setup。", this);
                yield break;
            }

            yield return new WaitForSeconds(thinkDelaySeconds);

            while (turnController.StepsFor(BattleSide.Enemy) > 0 && enemy.IsAlive && player.IsAlive)
            {
                var manhattan = Mathf.Abs(enemy.GridPosition.x - player.GridPosition.x)
                                + Mathf.Abs(enemy.GridPosition.y - player.GridPosition.y);

                if (manhattan == 1)
                {
                    player.ApplyDamage(hpDamage: 20, spDamage: 8);
                    turnController.TryConsumeStep(BattleSide.Enemy, 1);
                    Debug.Log($"Enemy attacks {player.DisplayName} (-20 HP, -8 SP)");
                    break;
                }

                var step = ChooseStepTowardsTarget(enemy.GridPosition, player.GridPosition);
                if (step == enemy.GridPosition)
                {
                    turnController.TryConsumeStep(BattleSide.Enemy, 1);
                    Debug.Log("Enemy cannot find valid move, consumes 1 step.");
                    continue;
                }

                enemy.MoveTo(step);
                turnController.TryConsumeStep(BattleSide.Enemy, 1);
                Debug.Log($"Enemy moves to {step}");
                yield return new WaitForSeconds(0.1f);
            }

            turnController.EndTurn();
        }

        private Vector2Int ChooseStepTowardsTarget(Vector2Int origin, Vector2Int target)
        {
            Vector2Int best = origin;
            var bestDist = int.MaxValue;

            foreach (var next in board.GetAdjacent(origin))
            {
                var dist = Mathf.Abs(next.x - target.x) + Mathf.Abs(next.y - target.y);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = next;
                }
            }

            return best;
        }
    }
}
