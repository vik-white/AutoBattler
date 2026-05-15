using Rukhanka.Toolbox;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using vikwhite.Data;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EventSystemGroup))]
    [UpdateBefore(typeof(CreatePrefabEventSystem))]
    public partial struct CreateVFXEventSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);

            foreach (var skillStartedEvent in SystemAPI.Query<RefRO<SkillStartedEvent>>())
            {
                var skillConfig = skillStartedEvent.ValueRO.Skill.Value;
                if (skillConfig.CastVFXPrefab == 0) continue;

                CreatePrefab(ecb, skillConfig.CastVFXPrefab, skillStartedEvent.ValueRO.Position);
            }

            foreach (var deadEvent in SystemAPI.Query<RefRO<DeadCharacterEvent>>())
            {
                var character = deadEvent.ValueRO.Character;
                if (!SystemAPI.HasComponent<Enemy>(character)) continue;
                if (!transforms.HasComponent(character)) continue;

                CreatePrefab(ecb, "DeadVFX".CalculateHash32(), transforms[character].Position);
            }

            foreach (var visualEvent in SystemAPI.Query<RefRO<SkillEffectVisualEvent>>())
            {
                var target = visualEvent.ValueRO.Target;
                var provider = visualEvent.ValueRO.Provider;
                if (!transforms.HasComponent(target) || !characters.HasComponent(target)) continue;

                var skillConfig = visualEvent.ValueRO.Skill.Value;
                if (skillConfig.VFXPrefab == 0) continue;

                CreatePrefab(ecb, skillConfig.VFXPrefab, GetEffectPosition(skillConfig, target, provider, transforms, characters));
            }

            ecb.Playback(state.EntityManager);
        }

        private static float3 GetEffectPosition(
            in SkillConfig skillConfig,
            Entity target,
            Entity provider,
            in ComponentLookup<LocalTransform> transforms,
            in ComponentLookup<Character> characters)
        {
            var targetPosition = transforms[target].Position;
            var targetConfig = characters[target].GetConfig();

            if (skillConfig.VFXSpawn == VFXSpawnType.Forward)
            {
                var position = targetPosition + new float3(0, 0.8f, 0);
                if (target == provider || !transforms.HasComponent(provider)) return position;

                var providerPosition = transforms[provider].Position;
                var direction = math.normalizesafe(providerPosition - targetPosition) * targetConfig.ColliderRadius;
                return targetPosition + new float3(direction.x, 0.8f, direction.z);
            }

            if (skillConfig.VFXSpawn == VFXSpawnType.Top)
                return targetPosition + new float3(0, targetConfig.Scale, 0);

            return targetPosition;
        }

        private static void CreatePrefab(EntityCommandBuffer ecb, uint id, float3 position)
        {
            ecb.CreateFrameEntity(new CreatePrefabEvent
            {
                ID = id,
                Position = position
            });
        }
    }
}
