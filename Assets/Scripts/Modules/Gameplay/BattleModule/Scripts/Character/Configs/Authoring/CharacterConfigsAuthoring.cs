using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using CapsuleCollider = Unity.Physics.CapsuleCollider;

namespace vikwhite.ECS
{
    public class CharacterConfigsAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
    }

    public class CharacterConfigsAuthoringBaker : Baker<CharacterConfigsAuthoring>
    {
        public override void Bake(CharacterConfigsAuthoring authoring) {  
            Debug.Log("CharacterConfigsAuthoringBaker");
            var entity = GetEntity(TransformUsageFlags.None);
            var entities = AddBuffer<CharacterConfig>(entity);
            var prefab = GetEntity(authoring.Prefab.ResetChildrenTransforms(), TransformUsageFlags.Dynamic);
            var collider = CapsuleCollider.Create(new CapsuleGeometry { Vertex0 = new float3(0, 0, 0), Vertex1 = new float3(0, 1, 0), Radius = 0.35f });
            entities.Add(new CharacterConfig {
                ID = CharacterID.IronfistDwarf,
                Prefab = prefab,
                Collider = collider,
            });
        } 
    }
}