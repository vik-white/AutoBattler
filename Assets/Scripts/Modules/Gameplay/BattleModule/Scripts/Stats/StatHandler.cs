using Unity.Entities;

namespace vikwhite.ECS
{
    public static class StatHandler
    {
        public static float Get(StatType type, DynamicBuffer<StatBase> statBase, DynamicBuffer<StatMultiply> statMultiply) => statBase[(int)type].Value + statMultiply[(int)type].Value;
    }
}