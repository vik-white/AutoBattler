using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct CharacterConfigInitializeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            var entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
            var configs = SystemAPI.GetSingletonBuffer<CharacterConfig>();
            for (int i = 0; i < configs.Length; i++)
            {
                var config = configs[i];
                var skinnedMeshRenderer = configs[i].GameObject.Value.GetComponentInChildren<SkinnedMeshRenderer>();
                config.MaterialID = entitiesGraphicsSystem.RegisterMaterial(skinnedMeshRenderer.sharedMaterial);
                config.MeshID = entitiesGraphicsSystem.RegisterMesh(skinnedMeshRenderer.sharedMesh);
                configs[i] = config;
            }
            state.Enabled = false;
        }
    }
}