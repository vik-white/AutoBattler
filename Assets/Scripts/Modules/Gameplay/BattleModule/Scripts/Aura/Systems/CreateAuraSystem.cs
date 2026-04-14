using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateAuraSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateAura>>()) {
                var ability = request.ValueRO.Ability;
                var aura = ecb.Instantiate(SystemAPI.GetSingletonBuffer<Prefab>()[ability.Prefab].Value);
                ecb.AddComponent<SceneEntity>(aura);
                ecb.AddComponent(aura, new Aura{ Interval = ability.AuraInterval });
                ecb.SetComponent(aura, new LocalTransform {
                    Position = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Provider).Position,
                    Rotation = quaternion.identity,
                    Scale = ability.AuraRadius
                });
                ecb.AddComponent(aura, new Provider{ Value = request.ValueRO.Provider });
                ecb.AddComponent(aura, new Effects{ Array = ability.Effects, Ability = new AbilityLevelData{ ID = ability.ID, Level = ability.Level } });
                ecb.AddComponent(aura, new Statuses{ Array = ability.Statuses, Ability = new AbilityLevelData{ ID = ability.ID, Level = ability.Level } });
                ecb.AddComponent(aura, new Stats{ Array = ability.Stats });
                ecb.AddComponent(aura, new DestroyTimer{ Time = ability.AuraLifetime });
                ecb.AddComponent<FollowingProvider>(aura);
                ecb.AddBuffer<CollisionTarget>(aura);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}