using Unity.Entities;

namespace vikwhite.ECS
{
    public struct GetDamageEvent : IComponentData
    {
        public Entity Character;
        public Entity Provider;
        public float Damage;
        public bool IsCrit;
    }
}
