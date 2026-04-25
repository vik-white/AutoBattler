using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct HealEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var healths = SystemAPI.GetComponentLookup<Health>();
            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAny<HealEffect>())
            {
                var character = target.ValueRO.Value;
                var config = SystemAPI.GetComponent<Character>(character).GetConfig();
                var health = healths[character].Value + effect.ValueRO.Value;
                if (health > config.Health) health = config.Health;
                healths[character] = new Health { Value = health };
            }
        }
    }
}
