using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class LocationStaticConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }

    public class LocationStaticConfigsAuthoringBaker : Baker<LocationStaticConfigsAuthoring>
    {
        public override void Bake(LocationStaticConfigsAuthoring authoring) {  
            var entity = GetEntity(TransformUsageFlags.None);
            var locations = new System.Collections.Generic.List<LocationStaticConfig>();

            foreach (var locationData in authoring.Configs.LocationStatic.GetAll())
            {
                var entities = new FixedList128Bytes<CharacterLevelData>();
                foreach (var enemy in locationData.Enemies)
                    entities.Add(enemy);
                
                locations.Add(new LocationStaticConfig
                {
                    ID = locationData.ID.CalculateHash32(),
                    Enemies = entities,
                });
            }

            AddComponent(entity, new LocationStaticConfigsBlob
            {
                Value = CreateLocationStaticBlob(locations)
            });
        }

        private BlobAssetReference<BlobArrayContainer<LocationStaticConfig>> CreateLocationStaticBlob(
            System.Collections.Generic.List<LocationStaticConfig> locations)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArrayContainer<LocationStaticConfig>>();
            var arrayBuilder = builder.Allocate(ref root.Array, locations.Count);
            for (int i = 0; i < locations.Count; i++)
                arrayBuilder[i] = locations[i];

            var blob = builder.CreateBlobAssetReference<BlobArrayContainer<LocationStaticConfig>>(Allocator.Persistent);
            AddBlobAsset(ref blob, out _);
            return blob;
        }
    }
}
