using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class CharacterConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
        public List<GameObject> Characters;
    }

    public class CharacterConfigsAuthoringBaker : Baker<CharacterConfigsAuthoring>
    {
        public override void Bake(CharacterConfigsAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            var runtimeData = AddBuffer<CharacterRenderData>(entity);

            foreach (var characterData in authoring.Configs.Characters.GetAll())
            {
                var prefab = GetCharacterPrefab(characterData, authoring.Characters);
                var config = CreateCharacterConfig(characterData, prefab);
                runtimeData.Add(new CharacterRenderData
                {
                    ID = config.ID,
                    Prefab = GetEntity(prefab.ResetChildrenTransforms(), TransformUsageFlags.Dynamic),
                    Config = CreateConfigBlob(config),
                    GameObject = prefab,
                });
            }
        }

        private CharacterConfigData CreateCharacterConfig(ICharacterData data, GameObject prefab)
        {
            var prefabCollider = prefab.GetComponent<UnityEngine.CapsuleCollider>();
            
            return new CharacterConfigData {
                ID = data.ID.CalculateHash32(),
                LevelUp = data.LevelUp.CalculateHash32(),
                StarLevelUp = data.StarLevelUp.CalculateHash32(),
                SkillLevelUp = data.SkillLevelUp.CalculateHash32(),
                Scale = data.Scale,
                Health = data.Health,
                Shield = data.Shield,
                Attack = data.Attack,
                Defense = data.Defense,
                CritChance = data.CritChance,
                CritValue = data.CritValue,
                HealthBar = data.HealthBar,
                ColliderRadius = prefabCollider.radius * data.Scale,
                ColliderHeight = prefabCollider.height * data.Scale,
                SkillAttack = data.SkillAttack,
                SkillActive = data.SkillActive,
                SkillPassive1 = data.SkillPassive1,
                SkillPassive2 = data.SkillPassive2,
                SkillMeta1 = data.SkillMeta1,
                SkillMeta2 = data.SkillMeta2,
                SkillMeta3 = data.SkillMeta3,
            };
        }

        private BlobAssetReference<CharacterConfigData> CreateConfigBlob(CharacterConfigData config)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<CharacterConfigData>();
            root = config;
            var blob = builder.CreateBlobAssetReference<CharacterConfigData>(Allocator.Persistent);
            AddBlobAsset(ref blob, out _);
            return blob;
        }

        private static GameObject GetCharacterPrefab(ICharacterData data, List<GameObject> characters)
        {
            foreach (var character in characters)
            {
                if (character.name != data.Prefab) continue;
                return character;
            }
            return null;
        }
    }
}
