using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    public partial struct CalculateStatsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var stats = SystemAPI.GetSingletonBuffer<StatMultiply>();
            for (int i = 0; i < stats.Length; i++) stats[i] = new StatMultiply();
            foreach (var change in SystemAPI.Query<RefRO<StatChange>>()) {
                var id = (int)change.ValueRO.Type;
                stats[id] = new StatMultiply{ Value = stats[id].Value + change.ValueRO.Value };
            }
        }
    }
}