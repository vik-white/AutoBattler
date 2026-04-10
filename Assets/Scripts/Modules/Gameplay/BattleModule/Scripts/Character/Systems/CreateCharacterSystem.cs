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
                ecb.AddComponent<Character>(character);
                var abilities = ecb.AddBuffer<Ability>(character);
                ecb.SetComponent(character, new LocalTransform{ Position = request.ValueRO.Position, Rotation = quaternion.identity, Scale = 1});
                var mass = PhysicsHandler.CreateFreezeRotationMass(request.ValueRO.Config.Collider.Value.MassProperties);
                if (!request.ValueRO.IsEnemy)
                {
                    mass.InverseMass = 0;
                    ecb.AddComponent(character, new Health{ Value = 500 });
                    abilities.Add(new Ability { Config = AbilityHandler.Get(AbilityID.RangeAttack, 0, SystemAPI.GetSingletonBuffer<AbilityConfig>()) });
                    ecb.CreateFrameEntity(new CreateCharacterEvent { Character = character });
                }
                else
                {
                    ecb.AddComponent<Enemy>(character);
                    ecb.AddComponent(character, new Health{ Value = 5 });
                    abilities.Add(new Ability { Config = AbilityHandler.Get(AbilityID.MeleeAttack, 0, SystemAPI.GetSingletonBuffer<AbilityConfig>()) });
                }
                ecb.SetComponent(character, mass);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}