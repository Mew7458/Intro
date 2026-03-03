using Intro.UnityPrototype.Core;
using Intro.UnityPrototype.Units;
using UnityEngine;
using UnityEngine.UI;

namespace Intro.UnityPrototype.UI
{
    public class BattleHudController : MonoBehaviour
    {
        [SerializeField] private TurnController turnController;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private Button attackButton;
        [SerializeField] private Text turnText;
        [SerializeField] private Text stepsText;
        [SerializeField] private Text playerStatsText;
        [SerializeField] private Text enemyStatsText;

        private PlayerInputController playerInput;
        private UnitModel player;
        private UnitModel enemy;

        public void Setup(PlayerInputController input, UnitModel playerUnit, UnitModel enemyUnit)
        {
            playerInput = input;
            player = playerUnit;
            enemy = enemyUnit;

            endTurnButton?.onClick.AddListener(EndTurn);
            attackButton?.onClick.AddListener(Attack);

            turnController.OnTurnChanged += Refresh;
            turnController.OnStepsChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (turnController != null)
            {
                turnController.OnTurnChanged -= Refresh;
                turnController.OnStepsChanged -= Refresh;
            }
        }

        private void EndTurn()
        {
            if (turnController.CurrentSide != BattleSide.Player) return;
            turnController.EndTurn();
        }

        private void Attack()
        {
            if (playerInput == null) return;
            if (turnController.CurrentSide != BattleSide.Player) return;
            playerInput.TryAttack();
            Refresh();
        }

        public void Refresh()
        {
            if (turnText != null)
            {
                turnText.text = $"Turn: {turnController.CurrentSide} | Round: {turnController.RoundCount + 1}";
            }

            if (stepsText != null)
            {
                stepsText.text = $"Player Steps: {turnController.PlayerSteps} | Enemy Steps: {turnController.EnemySteps}";
            }

            if (playerStatsText != null && player != null)
            {
                playerStatsText.text = $"Karma HP {player.Hp}/{player.MaxHp} | SP {player.Sp}/{player.MaxSp}";
            }

            if (enemyStatsText != null && enemy != null)
            {
                enemyStatsText.text = $"Lirathe HP {enemy.Hp}/{enemy.MaxHp} | SP {enemy.Sp}/{enemy.MaxSp}";
            }

            var playerTurn = turnController.CurrentSide == BattleSide.Player;
            if (endTurnButton != null) endTurnButton.interactable = playerTurn;
            if (attackButton != null) attackButton.interactable = playerTurn;
        }
    }
}
