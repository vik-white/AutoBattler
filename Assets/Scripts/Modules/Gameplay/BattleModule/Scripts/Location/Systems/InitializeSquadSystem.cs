using Unity.Entities;
using Unity.Mathematics;
using vikwhite.Utils;

namespace vikwhite.ECS
{
    
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public partial struct InitializeSquadSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (request, entity) in SystemAPI.Query<RefRO<InitializeSquad>>().WithEntityAccess())
            {
                for (int i = 0; i < request.ValueRO.Value.Length; i++)
                {
                    var character = request.ValueRO.Value[i];
                    if(character.ID == 0) continue;
                    ecb.CreateFrameEntity(new CreateCharacter
                    {
                        ID = character.ID, 
                        Level = character.Level,
                        Position = GetPosition(i)
                    });
                }
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }

        private float3 GetPosition(int index)
        {
            switch (index)
            {
                case 0: return HexCoordinatesHandler.AxialToWorld(new int2(-3,1));
                case 1: return HexCoordinatesHandler.AxialToWorld(new int2(-3,-1));
                case 2: return HexCoordinatesHandler.AxialToWorld(new int2(-4,1));
                case 3: return HexCoordinatesHandler.AxialToWorld(new int2(-3,0));
                case 4: return HexCoordinatesHandler.AxialToWorld(new int2(-3,-1));
                default: return float3.zero;
            }
        }
    }
}