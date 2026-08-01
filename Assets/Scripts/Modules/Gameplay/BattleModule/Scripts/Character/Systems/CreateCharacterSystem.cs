using System;
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
            var upgradeConfigData = SystemAPI.GetSingleton<UpgradeConfigsBlob>();
            var upgradeConfigs = upgradeConfigData.Value;
            foreach (var request in SystemAPI.Query<RefRW<CreateCharacter>>())
            {
                var renderData = renderDataBuffer.Get(request.ValueRO.ID);
                var config = renderData.Config.Value;
                var upgrade = new CharacterUpgrade
                {
                    LevelRank = math.max(0, request.ValueRO.Level - 1),
                    StarRank = request.ValueRO.Stars,
                    LevelUp = upgradeConfigs.Get(config.LevelUpgrade),
                    StarUp = upgradeConfigs.Get(config.StarUpgrade),
                    BreakthroughLevelPeriod = upgradeConfigData.BreakthroughLevelPeriod,
                    BreakthroughMultiply = upgradeConfigData.BreakthroughMultiply,
                };
                var baseHealth = config.Health;
                var baseAttack = config.Attack;
                var baseDefense = config.Defense;
                var useSummonHealth = request.ValueRO.SummonProvider != Entity.Null && config.SummonHealth > 0f;
                var useSummonAttack = request.ValueRO.SummonProvider != Entity.Null && config.SummonAttack > 0f;
                var useSummonDefense = request.ValueRO.SummonProvider != Entity.Null && config.SummonDefense > 0f;
                if (useSummonHealth || useSummonAttack || useSummonDefense)
                {
                    var providerCharacter = SystemAPI.GetComponent<Character>(request.ValueRO.SummonProvider);
                    var providerUpgrade = SystemAPI.GetComponent<CharacterUpgrade>(request.ValueRO.SummonProvider);
                    baseHealth = ResolveSummonBaseStat(
                        baseHealth,
                        config.SummonHealth,
                        providerCharacter,
                        providerUpgrade,
                        StatType.Health);
                    baseAttack = ResolveSummonBaseStat(
                        baseAttack,
                        config.SummonAttack,
                        providerCharacter,
                        providerUpgrade,
                        StatType.Attack);
                    baseDefense = ResolveSummonBaseStat(
                        baseDefense,
                        config.SummonDefense,
                        providerCharacter,
                        providerUpgrade,
                        StatType.Defense);
                }
                var character = new Character
                {
                    Config = renderData.Config,
                    Level = request.ValueRO.Level,
                    Stars = request.ValueRO.Stars,
                    SkillLevel = request.ValueRO.SkillLevel,
                    BaseHealth = baseHealth,
                    BaseAttack = baseAttack,
                    BaseDefense = baseDefense,
                    UseSummonHealth = useSummonHealth,
                    UseSummonAttack = useSummonAttack,
                    UseSummonDefense = useSummonDefense,
                };
                var characterEntity = ecb.Instantiate(renderData.Prefab);
                ecb.AddComponent<SceneEntity>(characterEntity);

                ecb.AddComponent(characterEntity, character);
                if (request.ValueRO.SquadCharacterID != 0)
                {
                    ecb.AddComponent(characterEntity, new SquadSelection
                    {
                        CharacterID = request.ValueRO.SquadCharacterID,
                        Slot = request.ValueRO.SquadSlot
                    });
                }
                if (request.ValueRO.IsEnemy) ecb.AddComponent<Enemy>(characterEntity);

                ecb.SetComponent(characterEntity, new LocalTransform
                {
                    Position = request.ValueRO.Position,
                    Rotation = GetInitialRotation(request.ValueRO.IsEnemy),
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

                var health = character.GetUpgradedBaseStat(upgrade, StatType.Health);
                ecb.AddComponent(characterEntity, new Defense{ Value = character.GetUpgradedBaseStat(upgrade, StatType.Defense) });
                ecb.AddComponent(characterEntity, new Health{ Value = health });
                ecb.AddComponent(characterEntity, new HealthMax{ Value = health });
                ecb.AddComponent(characterEntity, new Shield{ Value = config.Shield });
                ecb.AddComponent(characterEntity, new ShieldMax{ Value = config.Shield });
                ecb.AddComponent(characterEntity, new CritCounter{ Value = 0 });
                ecb.AddComponent(characterEntity, upgrade);

                var skills = ecb.AddBuffer<Skill>(characterEntity);
                ecb.AddBuffer<StarterSkill>(characterEntity);
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

        private static float ResolveSummonBaseStat(
            float ownBaseStat,
            float summonMultiplier,
            in Character providerCharacter,
            in CharacterUpgrade providerUpgrade,
            StatType stat)
        {
            return summonMultiplier > 0f
                ? providerCharacter.GetUpgradedBaseStat(providerUpgrade, stat) * summonMultiplier
                : ownBaseStat;
        }

        private static quaternion GetInitialRotation(bool isEnemy)
        {
            var direction = isEnemy ? new float3(-1f, 0f, 0f) : new float3(1f, 0f, 0f);
            return quaternion.LookRotationSafe(direction, math.up());
        }

        private void CreateSkill(DynamicBuffer<SkillRuntimeData> runtimeData, DynamicBuffer<Skill> skills, SkillSlotData<uint> slot)
        {
            if (slot.Value == 0) return;
            var configBlob = runtimeData.Get(slot.Value);
            var config = configBlob.Value;
            var cooldown = config.Trigger == TriggerType.BattleStart
                ? 0f
                : slot.Type == SkillSlotType.Attack
                    ? config.Cooldown
                    : config.Cooldown * 0.5f;
            skills.Add(new Skill
            {
                Config = configBlob,
                Cooldown = cooldown
            });
        }
    }
}
