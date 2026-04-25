using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class LocationFlowConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }
    
    public class LocationFlowConfigsAuthoringgBaker : Baker<LocationFlowConfigsAuthoring>
    {
        public override void Bake(LocationFlowConfigsAuthoring authoring) {  
            var entity = GetEntity(TransformUsageFlags.None);
            var locationConfigs = new List<(uint ID, List<LocationFlowStepData> Steps)>();

            var locations = new List<string>();
            foreach (var locationData in authoring.Configs.LocationFlow.GetAll())
            {
                if (!locations.Contains(locationData.LocationID)) locations.Add(locationData.LocationID);
            }
            
            foreach (var locationID in locations)
            {
                var steps = new List<LocationFlowStepData>();
                foreach (var locationData in authoring.Configs.LocationFlow.GetAll())
                {
                    if (locationData.LocationID != locationID) continue;
                    var enemies = new FixedList128Bytes<uint>();
                    foreach (var enemy in locationData.Enemies) enemies.Add(enemy.CalculateHash32());
                    steps.Add(new LocationFlowStepData
                    {
                        Time = locationData.Time,
                        Enemies = enemies,
                        Count = locationData.Count,
                        SpawnInterval = locationData.SpawnInterval,
                    });
                }

                locationConfigs.Add((locationID.CalculateHash32(), steps));
            }

            AddComponent(entity, new LocationFlowConfigsBlob
            {
                Value = CreateLocationFlowBlob(locationConfigs)
            });
        }

        private static BlobAssetReference<BlobArrayContainer<LocationFlowConfig>> CreateLocationFlowBlob(
            List<(uint ID, List<LocationFlowStepData> Steps)> locationConfigs)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArrayContainer<LocationFlowConfig>>();
            var arrayBuilder = builder.Allocate(ref root.Array, locationConfigs.Count);
            for (int i = 0; i < locationConfigs.Count; i++)
            {
                arrayBuilder[i].ID = locationConfigs[i].ID;
                var stepsBuilder = builder.Allocate(ref arrayBuilder[i].Steps, locationConfigs[i].Steps.Count);
                for (int j = 0; j < locationConfigs[i].Steps.Count; j++)
                {
                    stepsBuilder[j] = locationConfigs[i].Steps[j];
                }
            }
            return builder.CreateBlobAssetReference<BlobArrayContainer<LocationFlowConfig>>(Allocator.Persistent);
        }
    }
}
