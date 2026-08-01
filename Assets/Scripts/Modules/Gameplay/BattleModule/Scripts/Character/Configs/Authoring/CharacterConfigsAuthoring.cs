using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
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
                LevelUpgrade = data.LevelUpgrade.CalculateHash32(),
                StarUpgrade = data.StarUpgrade.CalculateHash32(),
                SkillUpgrade = data.SkillUpgrade.CalculateHash32(),
                Scale = data.Scale,
                Health = data.Health,
                Shield = data.Shield,
                Attack = data.Attack,
                Defense = data.Defense,
                SummonHealth = data.SummonHealth,
                SummonAttack = data.SummonAttack,
                SummonDefense = data.SummonDefense,
                CritChance = data.CritChance,
                CritValue = data.CritValue,
                HealthBar = data.HealthBar,
                ColliderRadius = prefabCollider.radius * data.Scale,
                ColliderHeight = prefabCollider.height * data.Scale,
                Skills = CreateSkillSlots(data.Skills),
            };
        }

        private static FixedList64Bytes<SkillSlotData<uint>> CreateSkillSlots(IReadOnlyDictionary<SkillSlotType, string> skills)
        {
            var list = new FixedList64Bytes<SkillSlotData<uint>>();
            if (skills == null) return list;
            foreach (var slot in SkillSlotExtensions.CharacterSlots)
            {
                if (!skills.TryGetValue(slot, out var id) || id == null) continue;
                list.Add(new SkillSlotData<uint> { Type = slot, Value = id.CalculateHash32() });
            }
            return list;
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
