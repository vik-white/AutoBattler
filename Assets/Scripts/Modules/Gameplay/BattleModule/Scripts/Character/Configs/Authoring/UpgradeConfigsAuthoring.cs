using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class UpgradeConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }

    public class UpgradeConfigsAuthoringBaker : Baker<UpgradeConfigsAuthoring>
    {
        public override void Bake(UpgradeConfigsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            var configs = new List<UpgradeConfig>();

            foreach (var levelUpData in authoring.Configs.Upgrades.GetAll())
            {
                configs.Add(new UpgradeConfig
                {
                    ID = levelUpData.ID.CalculateHash32(),
                    Health = levelUpData.Health,
                    Attack = levelUpData.Attack,
                    Defense = levelUpData.Defense,
                    CritChance = levelUpData.CritChance,
                    CritValue = levelUpData.CritValue,
                    SkillMultipliers = CreateSkillMultipliers(levelUpData.SkillMultipliers),
                });
            }

            AddComponent(entity, new LevelUpConfigsBlob
            {
                Value = CreateConfigsBlob(configs)
            });
        }

        private static FixedList64Bytes<SkillMultiplierData> CreateSkillMultipliers(IReadOnlyDictionary<SkillType, float> source)
        {
            var list = new FixedList64Bytes<SkillMultiplierData>();
            if (source == null) return list;
            foreach (var slot in SkillTypeExtensions.UpgradableSlots)
            {
                if (!source.TryGetValue(slot, out var value) || value == 0f) continue;
                list.Add(new SkillMultiplierData { Type = slot, Value = value });
            }
            return list;
        }

        private BlobAssetReference<BlobArrayContainer<UpgradeConfig>> CreateConfigsBlob(List<UpgradeConfig> configs)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<BlobArrayContainer<UpgradeConfig>>();
            var arrayBuilder = builder.Allocate(ref root.Array, configs.Count);
            for (int i = 0; i < configs.Count; i++)
                arrayBuilder[i] = configs[i];

            var blob = builder.CreateBlobAssetReference<BlobArrayContainer<UpgradeConfig>>(Allocator.Persistent);
            AddBlobAsset(ref blob, out _);
            return blob;
        }
    }
}
