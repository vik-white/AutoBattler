using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct ResurrectionEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deadCharacters = SystemAPI.GetComponentLookup<Dead>(true);
            var pendingResurrections = SystemAPI.GetComponentLookup<PendingResurrection>();
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            using var characters = new NativeList<Entity>(Allocator.Temp);
            using var healthPercentages = new NativeList<float>(Allocator.Temp);

            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAll<ResurrectionEffect>())
            {
                var character = target.ValueRO.Value;
                if (!deadCharacters.HasComponent(character)) continue;

                var healthPercentage = math.saturate(effect.ValueRO.Value);
                if (healthPercentage <= 0f) continue;

                if (pendingResurrections.HasComponent(character))
                {
                    var pendingResurrection = pendingResurrections[character];
                    if (healthPercentage > pendingResurrection.HealthPercentage)
                    {
                        pendingResurrection.HealthPercentage = healthPercentage;
                        pendingResurrections[character] = pendingResurrection;
                    }
                    continue;
                }

                AddOrUpdateRequest(characters, healthPercentages, character, healthPercentage);
            }

            for (var i = 0; i < characters.Length; i++)
            {
                var character = characters[i];
                ecb.AddComponent(character, new PendingResurrection
                {
                    HealthPercentage = healthPercentages[i]
                });

                if (state.EntityManager.HasComponent<Destroy>(character))
                    ecb.RemoveComponent<Destroy>(character);
            }

            ecb.Playback(state.EntityManager);
        }

        private static void AddOrUpdateRequest(
            NativeList<Entity> characters,
            NativeList<float> healthPercentages,
            Entity character,
            float healthPercentage)
        {
            for (var i = 0; i < characters.Length; i++)
            {
                if (characters[i] != character) continue;
                if (healthPercentage > healthPercentages[i])
                    healthPercentages[i] = healthPercentage;
                return;
            }

            characters.Add(character);
            healthPercentages.Add(healthPercentage);
        }
    }
}
