using Unity.Entities;
using Unity.Rendering;
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
            var runtimeData = SystemAPI.GetSingletonBuffer<CharacterRenderData>();
            for (int i = 0; i < runtimeData.Length; i++)
            {
                var config = runtimeData[i];
                var skinnedMeshRenderer = config.GameObject.Value.GetComponentInChildren<SkinnedMeshRenderer>();
                config.MaterialID = entitiesGraphicsSystem.RegisterMaterial(skinnedMeshRenderer.sharedMaterial);
                config.MeshID = entitiesGraphicsSystem.RegisterMesh(skinnedMeshRenderer.sharedMesh);
                runtimeData[i] = config;
            }
            state.Enabled = false;
        }
    }
}
