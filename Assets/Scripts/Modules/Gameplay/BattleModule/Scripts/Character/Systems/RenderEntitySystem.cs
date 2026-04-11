using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

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
                    if (!SystemAPI.HasBuffer<Child>(child.Value)) continue;
                    var subChildren = SystemAPI.GetBuffer<Child>(child.Value);
                    foreach (var subChild in subChildren)
                    {
                        if (state.EntityManager.HasComponent<RenderMeshArray>(subChild.Value))
                        {
                            var materialInfo = SystemAPI.GetComponent<MaterialMeshInfo>(subChild.Value);
                            ecb.AddComponent(entity, new RenderEntity { Entity = subChild.Value, Material = materialInfo.Material });
                            goto NextEntity;
                        }
                    }
                }
                NextEntity:;
            }
            ecb.Playback(state.EntityManager);
        }
    }
}