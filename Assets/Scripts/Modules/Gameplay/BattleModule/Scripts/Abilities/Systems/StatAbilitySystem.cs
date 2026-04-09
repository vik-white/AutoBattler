using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct StatAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var statAbility in SystemAPI.Query<RefRO<StatAbility>>()) {
                var ability = AbilityHandler.Get(statAbility.ValueRO.ID, statAbility.ValueRO.Level, SystemAPI.GetSingletonBuffer<AbilityConfig>());
                foreach (var stat in ability.Stats) 
                    ecb.CreateFrameEntity(new StatChange { ID = stat.ID, Value = stat.Value });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}