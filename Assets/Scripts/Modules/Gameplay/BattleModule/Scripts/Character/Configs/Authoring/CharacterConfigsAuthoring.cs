using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using UnityEngine;
using vikwhite.Data;
using CapsuleCollider = Unity.Physics.CapsuleCollider;

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
            Debug.Log("ECS CONFIGS UPDATED!");
            var entity = GetEntity(TransformUsageFlags.None);
            var entities = AddBuffer<CharacterConfig>(entity);
            
            foreach (var characterData in authoring.Configs.Characters.GetAll())
                entities.Add(CreateCharacterConfig(characterData, authoring.Characters));
        }

        private CharacterConfig CreateCharacterConfig(ICharacterData data, List<GameObject> characters)
        {
            GameObject prefab = null;
            foreach (var character in characters)
            {
                if(character.name != data.Prefab) continue;
                prefab = character;
                break;
            }
            var prefabCollider = prefab.GetComponent<UnityEngine.CapsuleCollider>();
            var entity = GetEntity(prefab.ResetChildrenTransforms(), TransformUsageFlags.Dynamic);
            
            var collider = CapsuleCollider.Create(new CapsuleGeometry
            {
                Vertex0 = new float3(0, 0, 0), 
                Vertex1 = new float3(0, prefabCollider.height * data.Scale, 0), 
                Radius = prefabCollider.radius * data.Scale
            });

            var abilities = new FixedList128Bytes<AbilityLevelData>();
            foreach(var ability in data.Abilities)
                abilities.Add(ability);
            
            return new CharacterConfig {
                ID = data.ID.CalculateHash32(),
                Prefab = entity,
                Collider = collider,
                Scale = data.Scale,
                Mass = data.Mass,
                Health = data.Health,
                Shield = data.Shield,
                HealthBar = data.HealthBar,
                ActiveAbility = data.ActiveAbility.CalculateHash32(),
                Abilities = abilities,
                ColliderRadius = prefabCollider.radius * data.Scale,
                GameObject = prefab,
            };
        }
    }
}