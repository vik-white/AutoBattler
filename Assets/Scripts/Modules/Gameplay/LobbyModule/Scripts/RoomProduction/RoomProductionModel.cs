using System;
using UnityEngine;

namespace vikwhite
{
    public class RoomProductionModel
    {
        public Room Room { get; }
        public ResourceType Type { get; }
        public Collider Anchor { get; }

        public RoomProductionModel(Room room, ResourceType type, Collider anchor)
        {
            Room = room;
            Type = type;
            Anchor = anchor;
        }
    }

    public readonly struct RoomProductionState
    {
        public float Accumulated { get; }
        public int CollectibleAmount => Mathf.FloorToInt(Accumulated);
        public int SecondsUntilNextProduction { get; }

        public RoomProductionState(float accumulated, int secondsUntilNextProduction)
        {
            Accumulated = accumulated;
            SecondsUntilNextProduction = secondsUntilNextProduction;
        }
    }

    public static class RoomProductionCalculator
    {
        public const long ProductionIntervalSeconds = 60;

        public static RoomProductionState Calculate(
            long lastCollectionUnixTime,
            float production,
            float capacity,
            long currentUnixTime)
        {
            var elapsedSeconds = Math.Max(0, currentUnixTime - lastCollectionUnixTime);
            var completedIntervals = elapsedSeconds / ProductionIntervalSeconds;
            var normalizedCapacity = Mathf.Max(0f, capacity);
            var accumulated = Mathf.Min(
                completedIntervals * Mathf.Max(0f, production),
                normalizedCapacity);
            var isAtCapacity = accumulated >= normalizedCapacity;
            var secondsUntilNextProduction = isAtCapacity
                ? 0
                : (int)(ProductionIntervalSeconds - elapsedSeconds % ProductionIntervalSeconds);

            return new RoomProductionState(
                accumulated,
                secondsUntilNextProduction);
        }
    }
}
