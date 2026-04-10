using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Character : IComponentData
    {
        public CharacterConfig Config;
    }
}