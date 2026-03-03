using Intro.UnityPrototype.Units;
using UnityEngine;

namespace Intro.UnityPrototype.Core
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private TurnController turnController;
        [SerializeField] private GridBoard board;

        private UnitModel player;
        private UnitModel enemy;

        public void Setup(UnitModel playerUnit, UnitModel enemyUnit)
        {
            player = playerUnit;
            enemy = enemyUnit;
        }

        private void Update()
        {
            if (player == null || enemy == null) return;
            if (!player.IsAlive || !enemy.IsAlive) return;
            if (turnController.CurrentSide != BattleSide.Player) return;

            if (Input.GetKeyDown(KeyCode.W)) TryMove(new Vector2Int(-1, 0));
            if (Input.GetKeyDown(KeyCode.S)) TryMove(new Vector2Int(1, 0));
            if (Input.GetKeyDown(KeyCode.A)) TryMove(new Vector2Int(0, -1));
            if (Input.GetKeyDown(KeyCode.D)) TryMove(new Vector2Int(0, 1));
            if (Input.GetKeyDown(KeyCode.Space)) TryAttack();
        }

        public bool TryMove(Vector2Int delta)
        {
            if (!turnController.TryConsumeStep(BattleSide.Player, 1)) return false;

            var next = player.GridPosition + delta;
            var blockedByEnemy = next == enemy.GridPosition && enemy.IsAlive;
            if (!board.IsWalkable(next) || blockedByEnemy)
            {
                Debug.Log("Move blocked.");
                return false;
            }

            player.MoveTo(next);
            Debug.Log($"Player moves to {next}");
            return true;
        }

        public bool TryAttack()
        {
            if (!turnController.TryConsumeStep(BattleSide.Player, 1)) return false;

            var dist = board.Manhattan(player.GridPosition, enemy.GridPosition);
            if (dist != 1)
            {
                Debug.Log("Enemy is not adjacent.");
                return false;
            }

            enemy.ApplyDamage(30, 10);
            Debug.Log("Player attacks Lirathe (-30 HP, -10 SP)");
            return true;
        }
    }
}
