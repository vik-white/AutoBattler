using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct AbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            foreach (var (abilities, transform, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>>().WithAll<Character>().WithEntityAccess()) 
            {
                var distance = float.MaxValue;
                if (SystemAPI.HasComponent<Target>(entity))
                {
                    var target = SystemAPI.GetComponent<Target>(entity);
                    var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.Value);
                    var direction = targetTransform.Position - transform.ValueRO.Position;
                    distance = math.length(direction);
                }
                
                for (int i = 0; i < abilities.Length; i++)
                {
                    ref var ability = ref abilities.ElementAt(i);
                    ability.IsReady = false;
                    ability.Cooldown += SystemAPI.GetSingleton<Time>().DeltaTime;
                    if (ability.Cooldown > ability.Config.Cooldown)
                    {
                        ability.IsReady = true;
                        ability.IsReady = distance <= ability.Config.Radius || ability.Config.Radius == 0;
                    }
                }
            }
        }
    }
}