using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAny<DamageEffect>())
            {
                var character = target.ValueRO.Value;
                var damage = effect.ValueRO.Value;
                if (damage > 0)
                {
                    var damageFlyTextPosition = GetDamageFlyTextPosition(transforms[character], characters[character].GetConfig());
                    ecb.CreateFrameEntity(new CreateDamageFlyTextEvent
                    {
                        Position = damageFlyTextPosition,
                        Damage = damage,
                        IsEnemyTarget = SystemAPI.HasComponent<Enemy>(character)
                    });
                }

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

                healths[character] = new Health { Value = healths[character].Value - damage };
            }
            ecb.Playback(state.EntityManager);
        }

        private static float3 GetDamageFlyTextPosition(LocalTransform transform, CharacterConfigData config)
        {
            var currentScale = math.max(transform.Scale, 0);
            var characterHeight = config.Scale > 0
                ? config.ColliderHeight * currentScale / config.Scale
                : config.ColliderHeight;

            return transform.Position + new float3(0, characterHeight, 0);
        }
    }
}
