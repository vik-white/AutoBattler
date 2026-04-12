using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using vikwhite.Data;
using CapsuleCollider = Unity.Physics.CapsuleCollider;

namespace vikwhite.ECS
{
    public class CharacterConfigsAuthoring : MonoBehaviour
    {
        public ConfigsLoader Configs;
    }

    public class CharacterConfigsAuthoringBaker : Baker<CharacterConfigsAuthoring>
    {
        public override void Bake(CharacterConfigsAuthoring authoring) {  
            Debug.Log("ECS CONFIGS UPDATED!");
            var entity = GetEntity(TransformUsageFlags.None);
            var entities = AddBuffer<CharacterConfig>(entity);

            foreach (var characterData in authoring.Configs.Characters.GetAll())
                entities.Add(CreateCharacterConfig(characterData));
        }

        private CharacterConfig CreateCharacterConfig(ICharacterData data)
        {
            var prefab = Resources.Load<GameObject>($"Characters/{data.Prefab}/{data.Prefab}");
            var entity = GetEntity(prefab.ResetChildrenTransforms(), TransformUsageFlags.Dynamic);
            var collider = CapsuleCollider.Create(new CapsuleGeometry { Vertex0 = new float3(0, 0, 0), Vertex1 = new float3(0, 1 * data.Scale, 0), Radius = 0.35f * data.Scale });

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
                HealthBar = data.HealthBar,
                ActiveAbility = data.ActiveAbility,
                Abilities = abilities,
            };
        }
    }
}