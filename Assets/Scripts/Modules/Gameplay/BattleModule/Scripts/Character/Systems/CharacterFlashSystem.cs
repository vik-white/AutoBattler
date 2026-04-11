using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct CharacterFlashSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (flash, renderEntity, entity) in SystemAPI.Query<RefRW<Flash>, RefRO<RenderEntity>>().WithEntityAccess())
            {
                flash.ValueRW.Value += dt;
                if (flash.ValueRO.Value > 0.1f)
                {
                    ecb.RemoveComponent<Flash>(entity);
                    var materialInfo = SystemAPI.GetComponent<MaterialMeshInfo>(renderEntity.ValueRO.Entity);
                    materialInfo.Material = renderEntity.ValueRO.Material;
                    ecb.SetComponent(renderEntity.ValueRO.Entity, materialInfo);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}