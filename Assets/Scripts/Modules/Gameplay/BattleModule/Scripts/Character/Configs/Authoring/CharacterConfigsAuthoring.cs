using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using CapsuleCollider = Unity.Physics.CapsuleCollider;

namespace vikwhite.ECS
{
    public class CharacterConfigsAuthoring : MonoBehaviour
    {
        public GameObject IronfistDwarf;
        public GameObject Sceleton;
    }

    public class CharacterConfigsAuthoringBaker : Baker<CharacterConfigsAuthoring>
    {
        public override void Bake(CharacterConfigsAuthoring authoring) {  
            Debug.Log("CharacterConfigsAuthoringBaker");
            var entity = GetEntity(TransformUsageFlags.None);
            var entities = AddBuffer<CharacterConfig>(entity);
            entities.Add(CreateCharacterConfig(CharacterID.IronfistDwarf, authoring.IronfistDwarf, 2500, 1f));
            entities.Add(CreateCharacterConfig(CharacterID.Sceleton, authoring.Sceleton, 5, 0.7f));
            entities.Add(CreateCharacterConfig(CharacterID.SceletonBoss, authoring.Sceleton, 50, 2f));
        }

        private CharacterConfig CreateCharacterConfig(CharacterID id, GameObject prefab, float health, float scale)
        {
            var entity = GetEntity(prefab.ResetChildrenTransforms(), TransformUsageFlags.Dynamic);
            var collider = CapsuleCollider.Create(new CapsuleGeometry { Vertex0 = new float3(0, 0, 0), Vertex1 = new float3(0, 1 * scale, 0), Radius = 0.35f * scale });
            return new CharacterConfig {
                ID = id,
                Prefab = entity,
                Collider = collider,
                Scale = scale,
                Health = health,
            };
        }
    }
}