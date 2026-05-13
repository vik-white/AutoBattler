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
        public FixedList64Bytes<SkillSlotData> Skills;

        public uint GetSkill(SkillSlotType slotType)
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                if (Skills[i].SlotType == slotType) return Skills[i].ID;
            }
            return 0;
        }

        public bool TryFindSlot(uint skillID, out SkillSlotType slotType)
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                if (Skills[i].ID == skillID)
                {
                    slotType = Skills[i].SlotType;
                    return true;
                }
            }
            slotType = SkillSlotType.None;
            return false;
        }
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
