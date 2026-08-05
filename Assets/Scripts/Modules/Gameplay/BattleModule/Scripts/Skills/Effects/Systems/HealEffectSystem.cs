using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct HealEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var healths = SystemAPI.GetComponentLookup<Health>();
            var healthMaxes = SystemAPI.GetComponentLookup<HealthMax>(true);
            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAny<HealEffect>())
            {
                var character = target.ValueRO.Value;
                var health = healths[character].Value + effect.ValueRO.Value;
                if (health > healthMaxes[character].Value) health = healthMaxes[character].Value;
                healths[character] = new Health { Value = health };
            }
        }
    }
}
