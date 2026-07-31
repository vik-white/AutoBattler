using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(StatusesSystemGroup))]
    [UpdateAfter(typeof(StatusDurationSystem))]
    public partial struct StunStatusSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var stunnedTargets = new NativeList<Entity>(Allocator.Temp);
            foreach (var (status, target) in SystemAPI.Query<RefRO<Status>, RefRO<Target>>().WithNone<Unapplied>())
            {
                if (status.ValueRO.Type != EffectType.Stun) continue;
                if (!Contains(stunnedTargets, target.ValueRO.Value))
                    stunnedTargets.Add(target.ValueRO.Value);
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (pendingSkills, entity) in SystemAPI.Query<DynamicBuffer<StarterSkill>>().WithAll<Character>().WithEntityAccess())
            {
                var shouldBeStunned = Contains(stunnedTargets, entity);
                var isStunned = SystemAPI.HasComponent<Stunned>(entity);

                if (shouldBeStunned)
                {
                    pendingSkills.Clear();

                    if (!isStunned)
                        ecb.AddComponent<Stunned>(entity);

                    if (SystemAPI.HasComponent<Jump>(entity))
                        ecb.RemoveComponent<Jump>(entity);

                    continue;
                }

                if (isStunned)
                    ecb.RemoveComponent<Stunned>(entity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            stunnedTargets.Dispose();
        }

        private static bool Contains(NativeList<Entity> entities, Entity entity)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] == entity)
                    return true;
            }

            return false;
        }
    }
}
