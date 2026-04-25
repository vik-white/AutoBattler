using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct InitializeSquad : IComponentData
    {
        public FixedList128Bytes<CreateCharacter> Value;
    }
}