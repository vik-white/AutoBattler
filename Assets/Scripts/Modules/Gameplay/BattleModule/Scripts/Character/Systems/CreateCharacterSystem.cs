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
            var abilityRuntimeData = SystemAPI.GetSingletonBuffer<SkillRuntimeData>(true);
            var renderDataBuffer = SystemAPI.GetSingletonBuffer<CharacterRenderData>(true);
            var upgradeConfigs = SystemAPI.GetSingleton<LevelUpConfigsBlob>().Value;
            foreach (var request in SystemAPI.Query<RefRW<CreateCharacter>>())
            {
                var renderData = renderDataBuffer.Get(request.ValueRO.ID);
                var config = renderData.Config.Value;
                var levelUpgradeConfig = upgradeConfigs.Get(config.LevelUpgrade);
                var starUpgradeConfig = upgradeConfigs.Get(config.StarUpgrade);
                var skillUpgradeConfig = upgradeConfigs.Get(config.SkillUpgrade);
                var healthMultiplier = 
                    CharacterHandler.GetLevelMultiplier(request.ValueRO.Level, levelUpgradeConfig.Health) *
                    CharacterHandler.GetLevelMultiplier(request.ValueRO.Stars, starUpgradeConfig.Health) *
                    CharacterHandler.GetLevelMultiplier(request.ValueRO.SkillLevel, skillUpgradeConfig.Health);
                var defenseMultiplier = 
                    CharacterHandler.GetLevelMultiplier(request.ValueRO.Level, levelUpgradeConfig.Defense) *
                    CharacterHandler.GetLevelMultiplier(request.ValueRO.Stars, starUpgradeConfig.Defense) *
                    CharacterHandler.GetLevelMultiplier(request.ValueRO.SkillLevel, skillUpgradeConfig.Defense);
                var characterEntity = ecb.Instantiate(renderData.Prefab);
                ecb.AddComponent<SceneEntity>(characterEntity);

                ecb.AddComponent(characterEntity, new Character
                {
                    Config = renderData.Config, 
                    Level = request.ValueRO.Level,
                    Stars = request.ValueRO.Stars,
                    SkillLevel = request.ValueRO.SkillLevel,
                });
                if (request.ValueRO.IsEnemy) ecb.AddComponent<Enemy>(characterEntity);

                ecb.SetComponent(characterEntity, new LocalTransform
                {
                    Position = request.ValueRO.Position,
                    Rotation = quaternion.identity,
                    Scale = config.Scale
                });
                ecb.AddComponent(characterEntity, new PreviousPosition { Value = request.ValueRO.Position });
                ecb.AddComponent<MoveDistance>(characterEntity);
                ecb.AddComponent<PathAvoidanceState>(characterEntity);
                ecb.AddComponent<ExternalVelocity>(characterEntity);

                var colliderRadius = config.Scale > 0 ? config.ColliderRadius / config.Scale : config.ColliderRadius;
                var colliderHeight = config.Scale > 0 ? config.ColliderHeight / config.Scale : config.ColliderHeight;
                var collider = CapsuleCollider.Create(new CapsuleGeometry
                {
                    Vertex0 = new float3(0, colliderRadius, 0),
                    Vertex1 = new float3(0, math.max(colliderRadius, colliderHeight - colliderRadius), 0),
                    Radius = colliderRadius
                });
                ecb.SetComponent(characterEntity, new PhysicsCollider { Value = collider });

                ecb.AddComponent(characterEntity, new Defense{ Value = config.Defense * defenseMultiplier });
                ecb.AddComponent(characterEntity, new Health{ Value = config.Health * healthMultiplier });
                ecb.AddComponent(characterEntity, new HealthMax{ Value = config.Health * healthMultiplier });
                ecb.AddComponent(characterEntity, new Shield{ Value = config.Shield });
                ecb.AddComponent(characterEntity, new ShieldMax{ Value = config.Shield });
                ecb.AddComponent(characterEntity, new CritCounter{ Value = 0 });

                var abilities = ecb.AddBuffer<Skill>(characterEntity);
                CreateAbility(abilityRuntimeData, abilities, config.SkillAttack, true);
                CreateAbility(abilityRuntimeData, abilities, config.SkillActive);
                CreateAbility(abilityRuntimeData, abilities, config.SkillPassive1);
                CreateAbility(abilityRuntimeData, abilities, config.SkillPassive2);
                CreateAbility(abilityRuntimeData, abilities, config.SkillMeta1);
                CreateAbility(abilityRuntimeData, abilities, config.SkillMeta2);
                CreateAbility(abilityRuntimeData, abilities, config.SkillMeta3);
                if(config.SkillActive != 0) ecb.AddComponent(characterEntity, new ActiveSkill{ Value = config.SkillActive });

                int statCount = Enum.GetValues(typeof(StatType)).Length;
                var statsBase = ecb.AddBuffer<StatBase>(characterEntity);
                for (int i = 1; i < statCount; i++) statsBase.Add(new StatBase { Value = 1 });

                var statsMultiply = ecb.AddBuffer<StatMultiply>(characterEntity);
                for (int i = 1; i < statCount; i++) statsMultiply.Add(new StatMultiply { Value = 1 });

                ecb.CreateFrameEntity(new CreateCharacterEvent { Character = characterEntity });
            }
            ecb.Playback(state.EntityManager);
        }

        private void CreateAbility(DynamicBuffer<SkillRuntimeData> abilityRuntimeData, DynamicBuffer<Skill> abilities, uint id, bool ready = false)
        {
            if(id == 0) return;
            var abilityConfigBlob = abilityRuntimeData.Get(id);
            var abilityConfig = abilityConfigBlob.Value;
            if (abilityConfig.Type == SkillType.Abilities)
            {
                foreach (var abilityChildID in abilityConfig.Skills)
                {
                    abilities.Add(new Skill { Config = abilityRuntimeData.Get(abilityChildID), IsChild = true });
                }
            }
            var cooldown = ready ? abilityConfig.Cooldown : abilityConfig.Cooldown * 0.5f;
            abilities.Add(new Skill { Config = abilityConfigBlob, Cooldown = cooldown });
        }
    }
}
