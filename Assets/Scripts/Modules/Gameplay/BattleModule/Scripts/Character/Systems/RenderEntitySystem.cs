using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct RenderEntitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var renderDataQuery = SystemAPI.QueryBuilder().WithAll<CharacterRenderData>().Build();
            if (renderDataQuery.IsEmptyIgnoreFilter) return;

            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var runtimeData = SystemAPI.GetSingletonBuffer<CharacterRenderData>(true);
            foreach (var (children, character, entity) in SystemAPI.Query<DynamicBuffer<Child>, RefRO<Character>>().WithNone<RenderEntity>().WithEntityAccess())
            {
                var renderEntities = ecb.AddBuffer<RenderEntity>(entity);
                var runtime = runtimeData.GetByConfig(character.ValueRO.Config);
                var materialInfo = new MaterialMeshInfo{
                    MaterialID = runtime.MaterialID,
                    MeshID  = runtime.MeshID,
                    SubMesh = 0,
                };
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
                    if (state.EntityManager.HasComponent<RenderMeshArray>(child.Value))
                    {
                        ecb.SetComponent(child.Value, materialInfo);
                        renderEntities.Add(new RenderEntity { Entity = child.Value, MaterialIndex = materialInfo.Material });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
