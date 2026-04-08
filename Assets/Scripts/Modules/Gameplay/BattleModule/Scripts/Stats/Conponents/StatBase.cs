using Unity.Entities;

namespace vikwhite.ECS
{
    public enum StatID
    {
        None = -1,
        DamageMultiply = 0,
        CooldownMultiply = 1,
    }
    
    public struct StatBase : IBufferElementData
    {
        public float Value;
    }
}