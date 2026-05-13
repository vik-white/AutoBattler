using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct CalculateStatsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            foreach (var (_, entity) in SystemAPI.Query<RefRO<Character>>().WithEntityAccess())
            {
                var stats = SystemAPI.GetBuffer<StatMultiply>(entity);
                var statBases = SystemAPI.GetBuffer<StatBase>(entity);
                for (int i = 0; i < stats.Length; i++) stats[i] = new StatMultiply{ Value = statBases[i].Value };
                
                foreach (var change in SystemAPI.Query<RefRO<StatChange>>()) {
                    if (change.ValueRO.Target == entity)
                    {
                        var id = (int)change.ValueRO.Type;
                        stats[id] = new StatMultiply{ Value = stats[id].Value + change.ValueRO.Value };
                    }
                }
            }
        }
    }
}