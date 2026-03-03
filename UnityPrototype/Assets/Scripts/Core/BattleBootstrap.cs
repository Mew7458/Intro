using System.Collections;
using Intro.UnityPrototype.AI;
using Intro.UnityPrototype.UI;
using Intro.UnityPrototype.Units;
using UnityEngine;

namespace Intro.UnityPrototype.Core
{
    public class BattleBootstrap : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GridBoard board;
        [SerializeField] private TurnController turnController;
        [SerializeField] private EnemyAIController enemyAI;
        [SerializeField] private PlayerInputController playerInput;
        [SerializeField] private BattleHudController hud;

        [Header("Optional Views")]
        [SerializeField] private UnitView playerView;
        [SerializeField] private UnitView enemyView;

        private UnitModel karma;
        private UnitModel lirathe;

        private void Start()
        {
            karma = new UnitModel("karma", "Karma", BattleSide.Player, new Vector2Int(4, 21), 200, 50);
            lirathe = new UnitModel("lirathe", "Lirathe", BattleSide.Enemy, new Vector2Int(4, 4), 700, 80);

            enemyAI.Setup(lirathe, karma);
            playerInput.Setup(karma, lirathe);
            hud?.Setup(playerInput, karma, lirathe);

            if (playerView != null) playerView.Bind(karma, board.GridToWorld);
            if (enemyView != null) enemyView.Bind(lirathe, board.GridToWorld);

            turnController.OnTurnChanged += HandleTurnChanged;
            turnController.StartBattle();
            Debug.Log("Battle started. Player turn. Controls: WASD move, Space attack.");
        }

        private void OnDestroy()
        {
            if (turnController != null)
            {
                turnController.OnTurnChanged -= HandleTurnChanged;
            }
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

            hud?.Refresh();
        }

        private IEnumerator RunEnemyTurn()
        {
            yield return enemyAI.ExecuteTurn();

            if (!karma.IsAlive)
            {
                Debug.Log("Karma defeated.");
            }

            hud?.Refresh();
        }

        // 兼容老调试入口
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
            playerInput.TryAttack();
            hud?.Refresh();
        }
    }
}
