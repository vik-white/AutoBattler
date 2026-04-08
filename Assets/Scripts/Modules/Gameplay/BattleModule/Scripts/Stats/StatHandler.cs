using Unity.Entities;

namespace vikwhite.ECS
{
    public static class StatHandler
    {
        public static float Get(StatID id, DynamicBuffer<StatBase> statBase, DynamicBuffer<StatMultiply> statMultiply) => statBase[(int)id].Value + statMultiply[(int)id].Value;
    }
}