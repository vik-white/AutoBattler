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
                var character = ecb.Instantiate(request.ValueRO.Config.Prefab);
                ecb.AddComponent(character, new Character { Config = request.ValueRO.Config });
                var abilities = ecb.AddBuffer<Ability>(character);
                ecb.SetComponent(character, new LocalTransform
                {
                    Position = request.ValueRO.Position, 
                    Rotation = quaternion.identity, 
                    Scale = request.ValueRO.Config.Scale
                });
                ecb.AddComponent(character, new Health{ Value = request.ValueRO.Config.Health });
                var mass = PhysicsHandler.CreateFreezeRotationMass(request.ValueRO.Config.Collider.Value.MassProperties);
                if (!request.ValueRO.IsEnemy)
                {
                    mass.InverseMass = 0.001f;
                    abilities.Add(new Ability { Config = AbilityHandler.Get(AbilityID.RangeAttack, 0, SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>()) });
                    abilities.Add(new Ability { Config = AbilityHandler.Get(AbilityID.OrbitingFireBoll, 0, SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>()) });
                    ecb.AddComponent(character, new ActiveAbility{ Value = AbilityID.OrbitingFireBoll });
                    ecb.CreateFrameEntity(new CreateCharacterEvent { Character = character });
                }
                else
                {
                    ecb.AddComponent<Enemy>(character);
                    abilities.Add(new Ability { Config = AbilityHandler.Get(AbilityID.MeleeAttack, 0, SystemAPI.GetSingletonBuffer<AbilityLevelsConfig>()) });
                    if(request.ValueRO.Config.ID == CharacterID.SceletonBoss) mass.InverseMass = 0.5f;
                }
                ecb.SetComponent(character, mass);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}