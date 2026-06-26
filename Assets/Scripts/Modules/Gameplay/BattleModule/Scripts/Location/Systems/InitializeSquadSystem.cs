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
                        Stars = character.Stars,
                        SkillLevel = character.SkillLevel,
                        Position = GetPosition(i)
                    });
                }
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }

        public static float3 GetPosition(int index)
        {
            return BattleSquadGridPositions.GetDefaultWorldPosition(index);
        }
    }

    public static class BattleSquadGridPositions
    {
        public const float DropSnapDistance = 0.6f;

        private static readonly int2[] AvailableCells =
        {
            new(-3, 0),
            new(-2, 0),
            new(-1, 0),
            new(-4, 1),
            new(-3, 1),
            new(-2, 1),
            new(-4, 2),
            new(-3, 2),
            new(-3, -1),
            new(-2, -1),
            new(-1, -1),
            new(-2, -2),
            new(-1, -2),
        };

        private static readonly int2[] DefaultCells =
        {
            new(-3, 1),
            new(-2, -1),
            new(-4, 1),
            new(-3, 0),
            new(-3, -1),
        };

        public static float3 GetDefaultWorldPosition(int slot)
        {
            if (slot < 0 || slot >= DefaultCells.Length) return float3.zero;
            return HexCoordinatesHandler.AxialToWorld(DefaultCells[slot]);
        }

        public static float3 GetWorldPosition(int2 cell)
        {
            return HexCoordinatesHandler.AxialToWorld(cell);
        }

        public static bool TryGetNearestCell(float3 worldPosition, out int2 cell, out float3 cellWorldPosition)
        {
            return TryGetNearestCell(worldPosition, DropSnapDistance, out cell, out cellWorldPosition);
        }

        public static bool TryGetNearestCell(float3 worldPosition, float maxDistance, out int2 cell, out float3 cellWorldPosition)
        {
            var nearestIndex = -1;
            var nearestDistanceSq = float.MaxValue;
            var positionXZ = worldPosition.xz;

            for (var i = 0; i < AvailableCells.Length; i++)
            {
                var candidateWorld = HexCoordinatesHandler.AxialToWorld(AvailableCells[i]);
                var distanceSq = math.distancesq(positionXZ, candidateWorld.xz);
                if (distanceSq >= nearestDistanceSq) continue;

                nearestDistanceSq = distanceSq;
                nearestIndex = i;
            }

            if (nearestIndex >= 0 && nearestDistanceSq <= maxDistance * maxDistance)
            {
                cell = AvailableCells[nearestIndex];
                cellWorldPosition = HexCoordinatesHandler.AxialToWorld(cell);
                return true;
            }

            cell = default;
            cellWorldPosition = default;
            return false;
        }

        public static bool SameCell(int2 first, int2 second)
        {
            return first.x == second.x && first.y == second.y;
        }
    }
}
