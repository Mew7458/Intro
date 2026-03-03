using System;
using Intro.UnityPrototype.Units;
using UnityEngine;

namespace Intro.UnityPrototype.Core
{
    public class TurnController : MonoBehaviour
    {
        [SerializeField] private int maxSteps = 10;
        [SerializeField] private int baseStartSteps = 3;

        public BattleSide CurrentSide { get; private set; } = BattleSide.Player;
        public int RoundCount { get; private set; }
        public int PlayerSteps { get; private set; }
        public int EnemySteps { get; private set; }

        public event Action OnTurnChanged;

        public void StartBattle()
        {
            RoundCount = 0;
            RefreshStepBudget();
            CurrentSide = BattleSide.Player;
            OnTurnChanged?.Invoke();
        }

        public int StepsFor(BattleSide side)
        {
            return side == BattleSide.Player ? PlayerSteps : EnemySteps;
        }

        public bool TryConsumeStep(BattleSide side, int count = 1)
        {
            if (count <= 0) return true;

            if (side == BattleSide.Player)
            {
                if (PlayerSteps < count) return false;
                PlayerSteps -= count;
                return true;
            }

            if (EnemySteps < count) return false;
            EnemySteps -= count;
            return true;
        }

        public void EndTurn()
        {
            CurrentSide = CurrentSide == BattleSide.Player ? BattleSide.Enemy : BattleSide.Player;

            if (CurrentSide == BattleSide.Player)
            {
                RoundCount++;
                RefreshStepBudget();
            }

            OnTurnChanged?.Invoke();
        }

        private void RefreshStepBudget()
        {
            var steps = Mathf.Min(maxSteps, baseStartSteps + RoundCount);
            PlayerSteps = steps;
            EnemySteps = steps;
        }
    }
}
