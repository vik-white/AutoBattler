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
}
