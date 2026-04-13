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
            var locationBuffer = AddBuffer<LocationFlowConfig>(entity);

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

                locationBuffer.Add(new LocationFlowConfig
                {
                    ID = locationID.CalculateHash32(),
                    Steps = ArrayHandler.CreateBlobArray(steps.Count, e => steps[e]),
                });
            }
        }
    }
}