using Unity.Entities;

namespace vikwhite.ECS
{
    public struct SquadConfig : IBufferElementData, IID
    {
        public uint ID { get; set; }
    }
}