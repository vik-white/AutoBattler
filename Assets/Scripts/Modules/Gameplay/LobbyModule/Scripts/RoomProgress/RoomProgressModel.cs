using UnityEngine;

namespace vikwhite
{
    public class RoomProgressModel
    {
        public Room Room { get; }
        public Collider Anchor { get; }

        public RoomProgressModel(Room room, Collider anchor)
        {
            Room = room;
            Anchor = anchor;
        }
    }
}
