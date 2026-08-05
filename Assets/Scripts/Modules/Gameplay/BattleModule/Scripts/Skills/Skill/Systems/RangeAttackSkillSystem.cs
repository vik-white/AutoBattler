using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct RangeAttackSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            var selectedTargets = SystemAPI.GetComponentLookup<Target>(true);
            var healths = SystemAPI.GetComponentLookup<Health>(true);
            var statMultipliers = SystemAPI.GetBufferLookup<StatMultiply>(true);
            foreach (var skillActivatedEvent in SystemAPI.Query<RefRO<SkillActivatedEvent>>()) {
                var skill = skillActivatedEvent.ValueRO.Skill;
                var config = skill.Value;
                var entity = skillActivatedEvent.ValueRO.Character;
                if (config.Type != SkillType.RangeAttack) continue;
                if (!transforms.HasComponent(entity)) continue;

                if (config.TargetsCount <= 0)
                {
                    if (!SkillHandler.TryGetTarget(skill, entity, skillActivatedEvent.ValueRO.TriggerSource, skillActivatedEvent.ValueRO.Trigger, selectedTargets, out var target))
                        continue;

                    CreateProjectile(ecb, skill, entity, target, transforms, characters);
                    continue;
                }

                using var enemies = SystemAPI.QueryBuilder().WithAll<Character>().WithAny<Enemy>().Build().ToEntityArray(Allocator.Temp);
                using var allies = SystemAPI.QueryBuilder().WithAll<Character>().WithNone<Enemy>().Build().ToEntityArray(Allocator.Temp);
                var targets = SkillHandler.GetTargets(
                    skill,
                    entity,
                    skillActivatedEvent.ValueRO.TriggerSource,
                    skillActivatedEvent.ValueRO.Trigger,
                    selectedTargets,
                    healths,
                    characters,
                    statMultipliers,
                    SystemAPI.HasComponent<Enemy>(entity),
                    enemies,
                    allies);

                foreach (var target in targets)
                    CreateProjectile(ecb, skill, entity, target, transforms, characters);
            }
            ecb.Playback(state.EntityManager);
        }

        private static void CreateProjectile(
            EntityCommandBuffer ecb,
            BlobAssetReference<SkillConfig> skill,
            Entity provider,
            Entity target,
            ComponentLookup<LocalTransform> transforms,
            ComponentLookup<Character> characters)
        {
            var transform = transforms[provider];
            var forward = math.mul(transform.Rotation, new float3(0, 0, 0.3f));
            var spawnPosition = transform.Position + forward;
            var rotation = transform.Rotation;

            if (transforms.HasComponent(target))
            {
                var projectileStartPosition = spawnPosition + new float3(0, 0.5f, 0);
                var targetPosition = transforms[target].Position;
                if (characters.HasComponent(target))
                {
                    var targetConfig = characters[target].GetConfig();
                    targetPosition.y += targetConfig.ColliderHeight * 0.5f;
                }

                var direction = targetPosition - projectileStartPosition;
                if (math.lengthsq(direction) > 0.0001f)
                    rotation = quaternion.LookRotationSafe(math.normalize(direction), math.up());
            }

            ecb.CreateFrameEntity(new CreateBulletProjectile
            {
                Skill = skill,
                Provider = provider,
                Position = spawnPosition,
                Rotation = rotation,
            });
        }
    }
}
