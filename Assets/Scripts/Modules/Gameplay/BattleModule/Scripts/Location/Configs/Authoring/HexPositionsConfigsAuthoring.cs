using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class HexPositionsConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }

    public class HexPositionsConfigsAuthoringBaker : Baker<HexPositionsConfigsAuthoring>
    {
        public override void Bake(HexPositionsConfigsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var positions = new List<HexPositionsConfig>();

            foreach (var hexPositionsData in authoring.Configs.HexPositions.GetAll())
            {
                positions.Add(new HexPositionsConfig
                {
                    ID = hexPositionsData.ID.CalculateHash32(),
                    C1 = hexPositionsData.C1,
                    C2 = hexPositionsData.C2,
                    C3 = hexPositionsData.C3,
                    C4 = hexPositionsData.C4,
                    C5 = hexPositionsData.C5,
                });
            }

            AddComponent(entity, new HexPositionsConfigsBlob
            {
                Value = CreateHexPositionsBlob(positions)
            });
        }

        private BlobAssetReference<BlobArrayContainer<HexPositionsConfig>> CreateHexPositionsBlob(
            List<HexPositionsConfig> positions)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArrayContainer<HexPositionsConfig>>();
            var arrayBuilder = builder.Allocate(ref root.Array, positions.Count);
            for (int i = 0; i < positions.Count; i++)
                arrayBuilder[i] = positions[i];

            var blob = builder.CreateBlobAssetReference<BlobArrayContainer<HexPositionsConfig>>(Allocator.Persistent);
            AddBlobAsset(ref blob, out _);
            return blob;
        }
    }
}
