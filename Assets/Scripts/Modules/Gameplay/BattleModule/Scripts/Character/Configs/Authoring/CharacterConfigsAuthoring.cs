using Unity.Entities;
using UnityEngine;

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
            entities.Add(new CharacterConfig {
                ID = CharacterID.IronfistDwarf,
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            });
        } 
    }
}