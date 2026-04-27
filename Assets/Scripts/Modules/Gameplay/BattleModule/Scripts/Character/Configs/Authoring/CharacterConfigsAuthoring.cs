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

            var abilities = new FixedList128Bytes<AbilityLevelData>();
            foreach (var ability in data.Abilities)
                abilities.Add(ability);

            return new CharacterConfigData {
                ID = data.ID.CalculateHash32(),
                LevelUp = data.LevelUp.CalculateHash32(),
                Scale = data.Scale,
                Mass = data.Mass,
                Health = data.Health,
                Shield = data.Shield,
                HealthBar = data.HealthBar,
                ActiveAbility = data.ActiveAbility.CalculateHash32(),
                Abilities = abilities,
                ColliderRadius = prefabCollider.radius * data.Scale,
                ColliderHeight = prefabCollider.height * data.Scale,
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
