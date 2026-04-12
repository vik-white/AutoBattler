using Rukhanka.Toolbox;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class SquadConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }
    
    public class SquadConfigsAuthoringBaker : Baker<SquadConfigsAuthoring>
    {
        public override void Bake(SquadConfigsAuthoring authoring) {  
            var entity = GetEntity(TransformUsageFlags.None);
            var squad = AddBuffer<SquadConfig>(entity);
            foreach (var squadData in authoring.Configs.Squad.GetAll())
                squad.Add(new SquadConfig { ID = squadData.ID.CalculateHash32() });
        }
    }
}