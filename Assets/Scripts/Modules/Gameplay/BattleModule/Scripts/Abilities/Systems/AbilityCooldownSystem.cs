using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct AbilityCooldownSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            foreach (var abilities in SystemAPI.Query<DynamicBuffer<Ability>>().WithAll<Character>()) {
                for (int i = 0; i < abilities.Length; i++)
                {
                    ref var ability = ref abilities.ElementAt(i);
                    ability.IsCooldown = false;
                    ability.Cooldown -= SystemAPI.GetSingleton<Time>().DeltaTime;
                    if (ability.Cooldown <= 0)
                    {
                        ability.IsCooldown = true;
                        ability.Cooldown = ability.Config.Cooldown;
                    }
                }
            }
        }
    }
}