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
                ecb.SetComponent(character, new LocalTransform{ Position = request.ValueRO.Position, Rotation = quaternion.identity, Scale = 1});
                ecb.SetComponent(character, PhysicsHandler.CreateFreezeRotationMass(request.ValueRO.Config.Collider.Value.MassProperties));
            }
            ecb.Playback(state.EntityManager);
        }
    }
}