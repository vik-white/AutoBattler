using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    public struct CharacterConfig : IBufferElementData
    {
        public CharacterID ID;
        public Entity Prefab;
        public BlobAssetReference<Collider> Collider;
        public float Scale;
        public float Health;
    }
}