using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct InitializeAbilitiesSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var abilities = ecb.AddBuffer<AbilityLevel>(ecb.CreateEntity());
            for (int i = 0; i < Enum.GetValues(typeof(AbilityID)).Length; i++)
                abilities.Add(new AbilityLevel { Value = -1 });
            
            /*var config = SystemAPI.GetSingleton<PlayerConfig>();
            for(int i = 0; i < config.Abilities.Value.Array.Length; i++) {
                var ability = config.Abilities.Value.Array[i];
                abilities[(int)ability.ID] = new AbilityLevel { Value = ability.Level };
                ecb.CreateFrameEntity(new CreateAbility { Value = ability.ID });
            }*/
            
            ecb.Playback(state.EntityManager);
            state.Enabled = false;
        }
    }
}