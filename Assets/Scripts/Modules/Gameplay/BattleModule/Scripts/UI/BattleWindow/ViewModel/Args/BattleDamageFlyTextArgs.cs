using UnityEngine;

namespace vikwhite
{
    public class BattleDamageFlyTextArgs
    {
        public Vector3 Position { get; }
        public float Damage { get; }
        public bool IsEnemyTarget { get; }
        public bool IsCrit { get; }

        public BattleDamageFlyTextArgs(Vector3 position, float damage, bool isEnemyTarget, bool isCrit)
        {
            Position = position;
            Damage = damage;
            IsEnemyTarget = isEnemyTarget;
            IsCrit = isCrit;
        }
    }
}
