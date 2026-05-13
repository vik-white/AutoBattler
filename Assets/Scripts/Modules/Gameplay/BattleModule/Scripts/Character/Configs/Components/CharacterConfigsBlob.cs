using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace vikwhite.ECS
{
    public struct LevelUpConfigsBlob : IComponentData
    {
        public BlobAssetReference<BlobArrayContainer<LevelUpConfig>> Value;
    }

    public struct CharacterConfigData : IID
    {
        public uint ID { get; set; }
        public uint LevelUp;
        public uint StarLevelUp;
        public uint SkillLevelUp;
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
        public uint SkillAttack;
        public uint SkillActive;
        public uint SkillPassive1;
        public uint SkillPassive2;
        public uint SkillMeta1;
        public uint SkillMeta2;
        public uint SkillMeta3;
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
