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
                var entities = new FixedList128Bytes<uint>();
                foreach (var enemy in locationData.Enemies)
                    entities.Add(enemy.CalculateHash32());
                
                locations.Add(new LocationStaticConfig
                {
                    ID = locationData.ID.CalculateHash32(),
                    Enemies = entities,
                });
            }

            AddComponent(entity, new LocationStaticConfigsBlob
            {
                Value = ArrayHandler.CreateBlobArray(locations.Count, i => locations[i])
            });
        }
    }
}
