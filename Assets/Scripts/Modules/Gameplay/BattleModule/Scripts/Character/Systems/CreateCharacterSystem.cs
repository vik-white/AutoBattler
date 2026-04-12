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
                ecb.SetComponent(character, new LocalTransform
                {
                    Position = request.ValueRO.Position, 
                    Rotation = quaternion.identity, 
                    Scale = config.Scale
                });
                ecb.AddComponent<SceneEntity>(character);
                ecb.AddComponent(character, new PreviousPosition { Value = request.ValueRO.Position });
                ecb.AddComponent(character, new Character { ID = config.ID });
                if (request.ValueRO.IsEnemy) ecb.AddComponent<Enemy>(character);
                ecb.AddComponent(character, new Health{ Value = config.Health });
                var abilities = ecb.AddBuffer<Ability>(character);
                foreach (var ability in config.Abilities)
                    abilities.Add(new Ability { Config = AbilityHandler.Get(ability.ID, ability.Level, SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>()) });
                if(config.ActiveAbility != AbilityID.None) ecb.AddComponent(character, new ActiveAbility{ Value = config.ActiveAbility });
                var mass = PhysicsHandler.CreateFreezeRotationMass(config.Collider.Value.MassProperties);
                mass.InverseMass = config.Mass;
                ecb.SetComponent(character, mass);
                ecb.CreateFrameEntity(new CreateCharacterEvent { Character = character });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}