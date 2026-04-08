using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct StatAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var abilityID in SystemAPI.Query<RefRO<StatAbility>>()) {
                var ability = AbilityHandler.Get(abilityID.ValueRO.ID, SystemAPI.GetSingletonBuffer<AbilityLevel>(), SystemAPI.GetSingletonBuffer<AbilityConfig>());
                foreach (var stat in ability.Stats) 
                    ecb.CreateFrameEntity(new StatChange { ID = stat.ID, Value = stat.Value });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}