using Unity.Entities;
using Unity.Rendering;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct CharacterFlashSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (flash, renderEntities, entity) in SystemAPI.Query<RefRW<Flash>, DynamicBuffer<RenderEntity>>().WithEntityAccess())
            {
                flash.ValueRW.Value += dt;
                if (flash.ValueRO.Value > 0.1f)
                {
                    ecb.RemoveComponent<Flash>(entity);

                    foreach (var renderEntity in renderEntities)
                    {
                        var materialInfo = SystemAPI.GetComponent<MaterialMeshInfo>(renderEntity.Entity);
                        materialInfo.Material = renderEntity.MaterialIndex;
                        ecb.SetComponent(renderEntity.Entity, materialInfo);
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}