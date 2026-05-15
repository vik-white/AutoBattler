using Rukhanka;
using Rukhanka.Toolbox;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    [UpdateAfter(typeof(SkillCooldownSystem))]
    public partial struct ActivateAnimatedSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var dead = SystemAPI.GetComponentLookup<Dead>(true);
            var attackHash = "Attack".CalculateHash32();

            foreach (var (pendingSkill, pendingEntity) in SystemAPI.Query<RefRO<PendingSkillActivation>>().WithEntityAccess())
            {
                var character = pendingSkill.ValueRO.Character;
                if (character == Entity.Null || dead.HasComponent(character))
                    ecb.DestroyEntity(pendingEntity);
            }

            foreach (var (events, character) in SystemAPI.Query<DynamicBuffer<AnimationEventComponent>>().WithEntityAccess())
            {
                if (dead.HasComponent(character)) continue;
                if (!HasAttackEvent(events, attackHash)) continue;

                foreach (var (pendingSkill, pendingEntity) in SystemAPI.Query<RefRO<PendingSkillActivation>>().WithEntityAccess())
                {
                    if (pendingSkill.ValueRO.Character != character) continue;

                    ecb.CreateFrameEntity(new SkillActivatedEvent
                    {
                        Character = character,
                        Skill = pendingSkill.ValueRO.Skill
                    });
                    ecb.DestroyEntity(pendingEntity);
                }
            }

            ecb.Playback(state.EntityManager);
        }

        private static bool HasAttackEvent(DynamicBuffer<AnimationEventComponent> events, uint attackHash)
        {
            foreach (var evnt in events)
            {
                if (evnt.nameHash == attackHash)
                    return true;
            }

            return false;
        }
    }
}
