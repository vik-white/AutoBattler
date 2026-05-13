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
