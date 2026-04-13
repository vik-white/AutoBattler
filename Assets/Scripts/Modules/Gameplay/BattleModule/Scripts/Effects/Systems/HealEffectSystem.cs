using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct HealEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var healths = SystemAPI.GetComponentLookup<Health>();
            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAny<EffectHeal>())
            {
                var character = target.ValueRO.Value;
                var characterID = SystemAPI.GetComponent<Character>(character).ID;
                var config = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(characterID);
                var health = healths[character].Value + effect.ValueRO.Value;
                if (health > config.Health) health = config.Health;
                healths[character] = new Health { Value = health };
            }
        }
    }
}