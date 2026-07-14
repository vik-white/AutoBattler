using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    public interface IRoomsService
    {
        void Initialize();
    }
    
    public class RoomsService : IRoomsService
    {
        private readonly IProfileService _profile;
        private List<Room> _rooms = new();
        
        public RoomsService(IProfileService profile)
        {
            _profile = profile;
        }

        public void Initialize()
        {
            foreach (var roomData in _profile.Data.Rooms)
                _rooms.Add(new Room(roomData.ID, roomData.Level));
        }
    }
}