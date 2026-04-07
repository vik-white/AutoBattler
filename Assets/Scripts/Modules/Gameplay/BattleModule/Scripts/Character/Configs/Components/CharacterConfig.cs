using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CharacterConfig : IBufferElementData
    {
        public CharacterID ID;
        public Entity Prefab;
    }
}