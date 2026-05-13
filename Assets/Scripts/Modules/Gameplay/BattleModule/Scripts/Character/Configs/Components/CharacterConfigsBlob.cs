using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace vikwhite.ECS
{
    public struct LevelUpConfigsBlob : IComponentData
    {
        public BlobAssetReference<BlobArrayContainer<UpgradeConfig>> Value;
    }

    public struct CharacterConfigData : IID
    {
        public uint ID { get; set; }
        public float Scale;
        public float Health;
        public float Shield;
        public float Attack;
        public float Defense;
        public float CritChance;
        public float CritValue;
        public bool HealthBar;
        public float ColliderRadius;
        public float ColliderHeight;
        public uint LevelUpgrade;
        public uint StarUpgrade;
        public uint SkillUpgrade;
        public FixedList64Bytes<SkillSlotData<uint>> Skills;

        public uint GetSkill(SkillSlotType slotType) => Skills.Get(slotType);

        public bool TryFindSlot(uint skillID, out SkillSlotType slotType) => Skills.TryFindSlot(skillID, out slotType);

        public bool TryGetStat(StatType stat, out float value)
        {
            switch (stat)
            {
                case StatType.Attack:     value = Attack;     return true;
                case StatType.Defense:    value = Defense;    return true;
                case StatType.Health:     value = Health;     return true;
                case StatType.CritChance: value = CritChance; return true;
                case StatType.CritValue:  value = CritValue;  return true;
                default:                  value = 0f;         return false;
            }
        }

        public float GetStat(StatType stat) => TryGetStat(stat, out var value) ? value : 0f;
    }

    public static class CharacterUpgradeExtensions
    {
        public static float GetStatMultiplier(int level, int stars, int skillLevel, StatType stat, in UpgradeConfig levelUp, in UpgradeConfig starUp, in UpgradeConfig skillUp) =>
            CharacterHandler.GetCompositeMultiplier(level, stars, skillLevel, levelUp.GetStatMultiplier(stat), starUp.GetStatMultiplier(stat), skillUp.GetStatMultiplier(stat));

        public static float GetSkillMultiplier(int level, int stars, int skillLevel, SkillSlotType slot, in UpgradeConfig levelUp, in UpgradeConfig starUp, in UpgradeConfig skillUp) =>
            CharacterHandler.GetCompositeMultiplier(level, stars, skillLevel, levelUp.GetSkillMultiplier(slot), starUp.GetSkillMultiplier(slot), skillUp.GetSkillMultiplier(slot));
    }

    public struct CharacterRenderData : IBufferElementData, IID
    {
        public uint ID { get; set; }
        public BlobAssetReference<CharacterConfigData> Config;
        public Entity Prefab;
        public UnityObjectRef<GameObject> GameObject;
        public BatchMaterialID MaterialID;
        public BatchMeshID MeshID;
    }

    public static class CharacterRenderDataExtensions
    {
        public static CharacterRenderData GetByConfig(this DynamicBuffer<CharacterRenderData> buffer, BlobAssetReference<CharacterConfigData> config)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Config == config)
                {
                    return buffer[i];
                }
            }
            return default;
        }
    }
}
