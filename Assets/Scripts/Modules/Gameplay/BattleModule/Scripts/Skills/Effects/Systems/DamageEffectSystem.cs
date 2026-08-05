using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct DamageEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var healths = SystemAPI.GetComponentLookup<Health>();
            var healthMaxes = SystemAPI.GetComponentLookup<HealthMax>(true);
            var defenses = SystemAPI.GetComponentLookup<Defense>();
            var shields = SystemAPI.GetComponentLookup<Shield>();
            var shieldMaxes = SystemAPI.GetComponentLookup<ShieldMax>();
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (effect, target, provider) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>, RefRO<Provider>>().WithAny<DamageEffect>())
            {
                var character = target.ValueRO.Value;
                var defense = defenses[character].Value;
                var damage = DamageHandler.CalculateDamage(effect.ValueRO.Value, defense);
                var receivedDamage = damage;

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
                        var characterConfig = SystemAPI.GetComponent<Character>(character).GetConfig();
                        shieldMaxes[character] = new ShieldMax { Value = characterConfig.Shield };
                    }
                }

                var previousHealth = healths[character].Value;
                var health = previousHealth - damage;
                var halfHealth = healthMaxes[character].Value * 0.5f;
                healths[character] = new Health { Value = health };

                if (receivedDamage > 0)
                {
                    ecb.CreateFrameEntity(new GetDamageEvent
                    {
                        Character = character,
                        Provider = provider.ValueRO.Value,
                        Damage = receivedDamage,
                        IsCrit = effect.ValueRO.IsCrit,
                        HealthDroppedBelowHalf = previousHealth >= halfHealth && health < halfHealth
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
