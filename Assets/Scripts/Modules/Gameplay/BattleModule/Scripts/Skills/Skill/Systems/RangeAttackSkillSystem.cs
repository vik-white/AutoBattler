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
            foreach (var (skills, transform, target, entity) in SystemAPI.Query<DynamicBuffer<Skill>, RefRO<LocalTransform>, RefRO<Target>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var skill in skills) {
                    if (!skill.TryGetActivatedConfig(SkillType.RangeAttack, out var config)) continue;

                    var forward = math.mul(transform.ValueRO.Rotation, new float3(0, 0, 0.3f));
                    var spawnPosition = transform.ValueRO.Position + forward;
                    var rotation = transform.ValueRO.Rotation;
                    var targetEntity = target.ValueRO.Value;

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
                        Skill = skill.Config, 
                        Provider = entity, 
                        Position = spawnPosition,
                        Rotation = rotation,
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
