using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using CapsuleCollider = Unity.Physics.CapsuleCollider;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateCharacterSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var abilityRuntimeData = SystemAPI.GetSingletonBuffer<AbilityRuntimeData>(true);
            var renderDataBuffer = SystemAPI.GetSingletonBuffer<CharacterRenderData>(true);
            foreach (var request in SystemAPI.Query<RefRW<CreateCharacter>>())
            {
                var renderData = renderDataBuffer.Get(request.ValueRO.ID);
                var config = renderData.Config.Value;
                var characterEntity = ecb.Instantiate(renderData.Prefab);
                ecb.AddComponent<SceneEntity>(characterEntity);

                ecb.AddComponent(characterEntity, new Character { Config = renderData.Config });
                if (request.ValueRO.IsEnemy) ecb.AddComponent<Enemy>(characterEntity);

                ecb.SetComponent(characterEntity, new LocalTransform
                {
                    Position = request.ValueRO.Position,
                    Rotation = quaternion.identity,
                    Scale = config.Scale
                });
                ecb.AddComponent(characterEntity, new PreviousPosition { Value = request.ValueRO.Position });
                ecb.AddComponent<MoveDistance>(characterEntity);

                var colliderRadius = config.Scale > 0 ? config.ColliderRadius / config.Scale : config.ColliderRadius;
                var colliderHeight = config.Scale > 0 ? config.ColliderHeight / config.Scale : config.ColliderHeight;
                var collider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(0, colliderRadius, 0),
                    Vertex1 = new float3(0, math.max(colliderRadius, colliderHeight - colliderRadius), 0),
                    Radius = colliderRadius
                });
                var mass = PhysicsHandler.CreateFreezeRotationMass(collider.Value.MassProperties);
                mass.InverseMass = config.Mass;
                ecb.SetComponent(characterEntity, mass);
                ecb.SetComponent(characterEntity, new PhysicsCollider { Value = collider });
                ecb.AddComponent<ExternalVelocity>(characterEntity);

                ecb.AddComponent(characterEntity, new Health{ Value = config.Health * CharacterHandler.GetLevelMultiplier(request.ValueRO.Level) });
                ecb.AddComponent(characterEntity, new HealthMax{ Value = config.Health * CharacterHandler.GetLevelMultiplier(request.ValueRO.Level) });
                ecb.AddComponent(characterEntity, new Shield{ Value = config.Shield });
                ecb.AddComponent(characterEntity, new ShieldMax{ Value = config.Shield });

                var abilities = ecb.AddBuffer<Ability>(characterEntity);
                foreach (var ability in config.Abilities)
                {
                    var abilityConfigBlob = abilityRuntimeData.Get(ability.ID, ability.Level);
                    var abilityConfig = abilityConfigBlob.Value;
                    if (abilityConfig.Type == AbilityType.Abilities)
                    {
                        foreach (var abilityChild in abilityConfig.Abilities)
                        {
                            abilities.Add(new Ability { Config = abilityRuntimeData.Get(abilityChild.ID, abilityChild.Level), IsChild = true });
                        }
                    }
                    var cooldown = config.ActiveAbility != ability.ID ? abilityConfig.Cooldown : 0;
                    abilities.Add(new Ability { Config = abilityConfigBlob, Cooldown = cooldown });
                }
                if(config.ActiveAbility != 0) ecb.AddComponent(characterEntity, new ActiveAbility{ Value = config.ActiveAbility });

                int statCount = Enum.GetValues(typeof(StatType)).Length;
                var statsBase = ecb.AddBuffer<StatBase>(characterEntity);
                for (int i = 1; i < statCount; i++) statsBase.Add(new StatBase { Value = 1 });

                var statsMultiply = ecb.AddBuffer<StatMultiply>(characterEntity);
                for (int i = 1; i < statCount; i++) statsMultiply.Add(new StatMultiply { Value = 1 });

                ecb.CreateFrameEntity(new CreateCharacterEvent { Character = characterEntity });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
