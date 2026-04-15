using Rukhanka.Toolbox;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(AOEEffectSystem))]
    public partial struct CreateEffectVFXSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var characterConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>();
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>())
            {
                var position = new float3(0, 0.5f, 0);
                if (request.ValueRO.Target != request.ValueRO.Provider)
                {
                    var characterPosition = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Target).Position;
                    var providerPosition = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Provider).Position;
                    var id = SystemAPI.GetComponent<Character>(request.ValueRO.Target).ID;
                    var config = characterConfig.Get(id);
                    var colliderRadius = config.ColliderRadius;
                    var direction = math.normalize(providerPosition - characterPosition) * colliderRadius;
                    position = characterPosition + new float3(direction.x, 0.5f, direction.z);
                }
                var isEnemy = SystemAPI.HasComponent<Enemy>(request.ValueRO.Target);
                var name = isEnemy ? "DamageVFX" : "DamageVFXRed";
                ecb.CreateFrameEntity(new CreatePrefabEvent { ID = name.CalculateHash32(), Position = position });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}