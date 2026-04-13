using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct InitializeStatsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            int count = Enum.GetValues(typeof(StatType)).Length;
            var entityBase = state.EntityManager.CreateEntity();
            var statsBase = state.EntityManager.AddBuffer<StatBase>(entityBase);
            for (int i = 1; i < count; i++) statsBase.Add(new StatBase { Value = 1 });
            
            var entityMultiply = state.EntityManager.CreateEntity();
            var statsMultiply = state.EntityManager.AddBuffer<StatMultiply>(entityMultiply);
            for (int i = 1; i < count; i++) statsMultiply.Add(new StatMultiply { Value = 1 });
            state.Enabled = false;
        }
    }
}