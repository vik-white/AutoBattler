using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct InitializeSquad : IComponentData
    {
        public FixedList512Bytes<CreateCharacter> Value;
    }
}