using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct DamageEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var healths = SystemAPI.GetComponentLookup<Health>();
            var shields = SystemAPI.GetComponentLookup<Shield>();
            var shieldMaxes = SystemAPI.GetComponentLookup<ShieldMax>();
            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAny<DamageEffect>())
            {
                var character = target.ValueRO.Value;
                var damage = effect.ValueRO.Value;
                var shield = shields[character].Value;
                if (shield > 0)
                {
                    shield -= damage;
                    if (shield < 0)
                    {
                        damage = -shield;
                        shield = 0;

                    }
                    else
                    {
                        damage = 0;
                    }
                    shields[character] = new Shield { Value = shield };
                    if (shield == 0)
                    {
                        var characterID = SystemAPI.GetComponent<Character>(character).ID;
                        var characterConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(characterID);
                        shieldMaxes[character] = new ShieldMax { Value = characterConfig.Shield };
                    }
                }
                
                healths[character] = new Health { Value = healths[character].Value - damage };
            }
        }
    }
}