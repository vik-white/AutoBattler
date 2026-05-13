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
            var skillRuntimeData = SystemAPI.GetSingletonBuffer<SkillRuntimeData>(true);
            var renderDataBuffer = SystemAPI.GetSingletonBuffer<CharacterRenderData>(true);
            var upgradeConfigs = SystemAPI.GetSingleton<LevelUpConfigsBlob>().Value;
            foreach (var request in SystemAPI.Query<RefRW<CreateCharacter>>())
            {
                var renderData = renderDataBuffer.Get(request.ValueRO.ID);
                var config = renderData.Config.Value;
                var upgrade = new CharacterUpgrade
                {
                    LevelRank = request.ValueRO.Level - 1,
                    StarRank = request.ValueRO.Stars,
                    SkillRank = request.ValueRO.SkillLevel - 1,
                    LevelUp = upgradeConfigs.Get(config.LevelUpgrade),
                    StarUp = upgradeConfigs.Get(config.StarUpgrade),
                    SkillUp = upgradeConfigs.Get(config.SkillUpgrade),
                };
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

                var health = config.Health * upgrade.GetStatMultiplier(StatType.Health);
                ecb.AddComponent(characterEntity, new Defense{ Value = config.Defense * upgrade.GetStatMultiplier(StatType.Defense) });
                ecb.AddComponent(characterEntity, new Health{ Value = health });
                ecb.AddComponent(characterEntity, new HealthMax{ Value = health });
                ecb.AddComponent(characterEntity, new Shield{ Value = config.Shield });
                ecb.AddComponent(characterEntity, new ShieldMax{ Value = config.Shield });
                ecb.AddComponent(characterEntity, new CritCounter{ Value = 0 });
                ecb.AddComponent(characterEntity, upgrade);

                var skills = ecb.AddBuffer<Skill>(characterEntity);
                for (int i = 0; i < config.Skills.Length; i++)
                    CreateSkill(skillRuntimeData, skills, config.Skills[i]);

                int statCount = Enum.GetValues(typeof(StatType)).Length;
                var statsBase = ecb.AddBuffer<StatBase>(characterEntity);
                for (int i = 1; i < statCount; i++) statsBase.Add(new StatBase { Value = 1 });

                var statsMultiply = ecb.AddBuffer<StatMultiply>(characterEntity);
                for (int i = 1; i < statCount; i++) statsMultiply.Add(new StatMultiply { Value = 1 });

                ecb.CreateFrameEntity(new CreateCharacterEvent { Character = characterEntity });
            }
            ecb.Playback(state.EntityManager);
        }

        private void CreateSkill(DynamicBuffer<SkillRuntimeData> runtimeData, DynamicBuffer<Skill> skills, SkillSlotData<uint> slot)
        {
            if (slot.Value == 0) return;
            var configBlob = runtimeData.Get(slot.Value);
            var config = configBlob.Value;
            if (config.Type == SkillType.Skills)
            {
                foreach (var childID in config.Skills)
                    skills.Add(new Skill { Config = runtimeData.Get(childID), IsChild = true });
            }
            var cooldown = slot.Type == SkillSlotType.Attack ? config.Cooldown : config.Cooldown * 0.5f;
            skills.Add(new Skill { Config = configBlob, Cooldown = cooldown });
        }
    }
}
