using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateCharacterSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var request in SystemAPI.Query<RefRW<CreateCharacter>>())
            {
                var config = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(request.ValueRO.ID);
                var character = ecb.Instantiate(config.Prefab);
                ecb.AddComponent<SceneEntity>(character);
                
                ecb.AddComponent(character, new Character { ID = config.ID });
                if (request.ValueRO.IsEnemy) ecb.AddComponent<Enemy>(character);
                
                ecb.SetComponent(character, new LocalTransform
                {
                    Position = request.ValueRO.Position, 
                    Rotation = quaternion.identity, 
                    Scale = config.Scale
                });
                ecb.AddComponent(character, new PreviousPosition { Value = request.ValueRO.Position });
                ecb.AddComponent<MoveDistance>(character);
                
                var mass = PhysicsHandler.CreateFreezeRotationMass(config.Collider.Value.MassProperties);
                mass.InverseMass = config.Mass;
                ecb.SetComponent(character, mass);
                ecb.AddComponent<ExternalVelocity>(character);
                
                ecb.AddComponent(character, new Health{ Value = config.Health });
                ecb.AddComponent(character, new Shield{ Value = config.Shield });
                ecb.AddComponent(character, new ShieldMax{ Value = config.Shield });
                
                var abilities = ecb.AddBuffer<Ability>(character);
                var abilityBuffer = SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>();
                foreach (var ability in config.Abilities)
                {
                    var abilityConfig = abilityBuffer.Get(ability.ID).Levels.Value.Array[ability.Level];
                    if (abilityConfig.Type == AbilityType.Abilities)
                    {
                        foreach (var abilityChild in abilityConfig.Abilities)
                        {
                            var abilityChildConfig = abilityBuffer.Get(abilityChild.ID).Levels.Value.Array[abilityChild.Level];
                            abilities.Add(new Ability { Config = abilityChildConfig, IsChild = true });
                        }    
                    }
                    abilities.Add(new Ability { Config = abilityConfig });
                }
                if(config.ActiveAbility != 0) ecb.AddComponent(character, new ActiveAbility{ Value = config.ActiveAbility });
                
                int statCount = Enum.GetValues(typeof(StatType)).Length;
                var statsBase = ecb.AddBuffer<StatBase>(character);
                for (int i = 1; i < statCount; i++) statsBase.Add(new StatBase { Value = 1 });
            
                var statsMultiply = ecb.AddBuffer<StatMultiply>(character);
                for (int i = 1; i < statCount; i++) statsMultiply.Add(new StatMultiply { Value = 1 });
                
                ecb.CreateFrameEntity(new CreateCharacterEvent { Character = character });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}