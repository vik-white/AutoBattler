using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine.Rendering;

namespace vikwhite.ECS
{
    public struct CharacterConfig : IBufferElementData, IID
    {
        public uint ID { get; set; }
        public Entity Prefab;
        public BlobAssetReference<Collider> Collider;
        public float Scale;
        public float Mass;
        public float Health;
        public float Shield;
        public bool HealthBar;
        public uint ActiveAbility;
        public FixedList128Bytes<AbilityLevelData> Abilities;
        public float ColliderRadius;
        
        public BatchMaterialID MaterialID;
        public BatchMeshID MeshID;
    }
}