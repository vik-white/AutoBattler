using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct RearJumpSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var enemies = SystemAPI.GetComponentLookup<Enemy>(true);
            var dead = SystemAPI.GetComponentLookup<Dead>(true);
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);

            foreach (var skillActivatedEvent in SystemAPI.Query<RefRO<SkillActivatedEvent>>())
            {
                var skill = skillActivatedEvent.ValueRO.Skill;
                var entity = skillActivatedEvent.ValueRO.Character;
                if (skill.Value.Type != SkillType.RearJump) continue;
                if (dead.HasComponent(entity)) continue;
                if (SystemAPI.HasComponent<Jump>(entity)) continue;
                if (!transforms.HasComponent(entity)) continue;

                var target = Entity.Null;
                var maxDistanceSq = float.MinValue;
                var isEnemy = enemies.HasComponent(entity);
                var position = transforms[entity].Position;

                foreach (var (otherTransform, otherEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Character>().WithNone<Dead>().WithEntityAccess())
                {
                    if (otherEntity == entity || dead.HasComponent(otherEntity)) continue;
                    if (isEnemy == enemies.HasComponent(otherEntity)) continue;

                    float distanceSq = math.distancesq(position, otherTransform.ValueRO.Position);
                    if (distanceSq <= maxDistanceSq) continue;

                    maxDistanceSq = distanceSq;
                    target = otherEntity;
                }

                if (target == Entity.Null) continue;

                float distance = math.distance(position, transforms[target].Position);
                ecb.AddComponent(entity, new Jump
                {
                    Value = target,
                    StartPosition = position,
                    Progress = 0f,
                    Duration = math.max(distance / 8f, 0.2f),
                    Height = math.max(1.5f, distance * 0.2f)
                });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
