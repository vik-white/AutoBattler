using System;
using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    public class TavernHierarchy : MonoBehaviour
    {
        public List<RoomContainer> Rooms;
    }

    [Serializable]
    public class RoomContainer
    {
        public RoomType Type;
        public Transform Container;
    }
}