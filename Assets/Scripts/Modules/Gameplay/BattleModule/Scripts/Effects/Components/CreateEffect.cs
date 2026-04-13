using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    public struct CreateEffect : IComponentData
    {
        public Entity Target;
        public EffectData Data;
    }
}