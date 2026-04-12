using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct StartStaticLocationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (request, entity) in SystemAPI.Query<RefRW<StartStaticLocation>>().WithEntityAccess())
            {
                var squadConfig = SystemAPI.GetSingletonBuffer<SquadConfig>();
                var locationConfig = SystemAPI.GetSingletonBuffer<LocationStaticConfig>().Get(request.ValueRO.ID);

                foreach (var character in squadConfig)
                    ecb.CreateFrameEntity(new CreateCharacter{ID = character.ID, Position = new float3(-4, 0, 0)});
                
                foreach (var enemy in locationConfig.Enemies)
                    ecb.CreateFrameEntity(new CreateCharacter{ID = enemy, Position = new float3(4, 0, 0), IsEnemy = true});
                
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}