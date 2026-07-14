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
        private readonly IRoomFactory _roomFactory;
        private readonly IRoomSelectionService _roomSelection;
        
        public RoomsService(
            IProfileService profile,
            IConfigs configs,
            IRoomFactory roomFactory,
            IRoomSelectionService roomSelection)
        {
            _profile = profile;
            _configs = configs;
            _roomFactory = roomFactory;
            _roomSelection = roomSelection;
        }

        public void Initialize()
        {
            _roomSelection.Clear();

            var tavern = UnityEngine.Object.FindAnyObjectByType<TavernHierarchy>(FindObjectsInactive.Include);
            foreach (var roomContainer in tavern.Rooms)
            {
                roomContainer.Container.ClearChildren();
                var profileData = _profile.Data.Rooms.Find(e => e.Type == roomContainer.Type);
                var configData = _configs.Rooms.GetAll().Find(e => e.Type == roomContainer.Type);
                var roomModel = _roomFactory.Create(profileData.Type, profileData.Level);
                _roomSelection.Register(roomContainer.Collider, roomModel);

                var roomGO = GameObject.Instantiate(configData.Prefab, roomContainer.Container);
                roomGO.transform.localPosition = Vector3.zero;
            }
        }
    }
}
