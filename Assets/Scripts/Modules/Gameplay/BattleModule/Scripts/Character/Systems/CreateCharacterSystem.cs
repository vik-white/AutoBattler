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
            foreach (var request in SystemAPI.Query<RefRO<CreateCharacter>>())
            {
                var character = ecb.Instantiate(request.ValueRO.Config.Prefab);
                ecb.AddComponent<Character>(character);
                if (request.ValueRO.IsEnemy) ecb.AddComponent<Enemy>(character);
                ecb.SetComponent(character, new LocalTransform{ Position = request.ValueRO.Position, Rotation = quaternion.identity, Scale = 1});
                var mass = PhysicsHandler.CreateFreezeRotationMass(request.ValueRO.Config.Collider.Value.MassProperties);
                if (!request.ValueRO.IsEnemy)
                {
                    mass.InverseMass = 0;
                    ecb.CreateFrameEntity(new CreateAbility { Provider = character, ID = AbilityID.RangeAttack, Level = 0 });
                }
                ecb.SetComponent(character, mass);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}