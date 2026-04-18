using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct AuraAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, entity) in SystemAPI.Query<DynamicBuffer<Ability>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    if (ability.Config.Type != AbilityType.Aura || !ability.IsActivate) continue;
                    ecb.CreateFrameEntity(new CreateAura()
                    {
                        Provider = entity,
                        Ability = ability.Config,
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}