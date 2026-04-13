using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct LocationFlowStepData
    {
        public float Time;
        public FixedList128Bytes<uint> Enemies;
        public int Count;
        public float SpawnInterval;
    }
}