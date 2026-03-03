using UnityEngine;

namespace Intro.UnityPrototype.Units
{
    public enum BattleSide
    {
        Player,
        Enemy,
    }

    [System.Serializable]
    public class UnitModel
    {
        public string Id;
        public string DisplayName;
        public BattleSide Side;

        public Vector2Int GridPosition;

        public int MaxHp;
        public int Hp;

        public int MaxSp;
        public int Sp;

        public bool IsAlive => Hp > 0;

        public UnitModel(string id, string displayName, BattleSide side, Vector2Int gridPosition, int maxHp, int maxSp)
        {
            Id = id;
            DisplayName = displayName;
            Side = side;
            GridPosition = gridPosition;
            MaxHp = maxHp;
            Hp = maxHp;
            MaxSp = maxSp;
            Sp = maxSp;
        }

        public void MoveTo(Vector2Int targetCell)
        {
            GridPosition = targetCell;
        }

        public void ApplyDamage(int hpDamage, int spDamage)
        {
            Hp = Mathf.Clamp(Hp - Mathf.Max(0, hpDamage), 0, MaxHp);
            Sp = Mathf.Clamp(Sp - Mathf.Max(0, spDamage), 0, MaxSp);
        }
    }
}
