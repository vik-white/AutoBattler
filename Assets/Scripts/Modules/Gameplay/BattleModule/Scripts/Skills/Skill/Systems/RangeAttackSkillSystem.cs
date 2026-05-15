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
            var targets = SystemAPI.GetComponentLookup<Target>(true);
            foreach (var skillActivatedEvent in SystemAPI.Query<RefRO<SkillActivatedEvent>>()) {
                var skill = skillActivatedEvent.ValueRO.Skill;
                var config = skill.Value;
                var entity = skillActivatedEvent.ValueRO.Character;
                if (config.Type != SkillType.RangeAttack) continue;
                if (!transforms.HasComponent(entity)) continue;
                if (!SkillHandler.TryGetTarget(skill, entity, skillActivatedEvent.ValueRO.Trigger, targets, out var targetEntity)) continue;

                var transform = transforms[entity];
                var forward = math.mul(transform.Rotation, new float3(0, 0, 0.3f));
                var spawnPosition = transform.Position + forward;
                var rotation = transform.Rotation;

                if (transforms.HasComponent(targetEntity))
                {
                    var projectileStartPosition = spawnPosition + new float3(0, 0.5f, 0);
                    var targetPosition = transforms[targetEntity].Position;
                    if (characters.HasComponent(targetEntity))
                    {
                        var targetConfig = characters[targetEntity].GetConfig();
                        targetPosition.y += targetConfig.ColliderHeight * 0.5f;
                    }

                    var direction = targetPosition - projectileStartPosition;
                    if (math.lengthsq(direction) > 0.0001f)
                        rotation = quaternion.LookRotationSafe(math.normalize(direction), math.up());
                }

                ecb.CreateFrameEntity(new CreateBulletProjectile
                {
                    Skill = skill,
                    Provider = entity,
                    Position = spawnPosition,
                    Rotation = rotation,
                });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
