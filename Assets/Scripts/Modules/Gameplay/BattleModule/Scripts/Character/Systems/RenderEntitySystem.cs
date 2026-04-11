using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct RenderEntitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (children, entity) in SystemAPI.Query<DynamicBuffer<Child>>().WithAny<Character>().WithNone<RenderEntity>().WithEntityAccess())
            {
                foreach (var child in children)
                {
                    if (state.EntityManager.HasComponent<RenderMeshArray>(child.Value))
                    {
                        var materialInfo = SystemAPI.GetComponent<MaterialMeshInfo>(child.Value);
                        ecb.AddComponent(entity, new RenderEntity { Entity = child.Value, Material = materialInfo.Material });
                        goto NextEntity;
                    }
                }
                NextEntity:;
            }
            ecb.Playback(state.EntityManager);
        }
    }
}