using System.Collections.Generic;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRoomsService
    {
        void Initialize();
    }
    
    public class RoomsService : IRoomsService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private List<Room> _rooms = new();
        
        public RoomsService(IProfileService profile, IConfigs configs)
        {
            _profile = profile;
            _configs = configs;
        }

        public void Initialize()
        {
            var tavern = UnityEngine.Object.FindAnyObjectByType<TavernHierarchy>(FindObjectsInactive.Include);
            foreach (var room in tavern.Rooms)
            {
                room.Container.ClearChildren();
                var profileData = _profile.Data.Rooms.Find(e => e.Type == room.Type);
                var configData = _configs.Rooms.GetAll().Find(e => e.Type == room.Type);
                _rooms.Add(new Room(profileData.Type, profileData.Level));
                var roomGO = GameObject.Instantiate(configData.Prefab, room.Container);
                roomGO.transform.localPosition = Vector3.zero;
            }
        }
    }
}