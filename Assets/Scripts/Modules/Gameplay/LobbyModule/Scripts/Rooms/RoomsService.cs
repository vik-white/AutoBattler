using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRoomsService
    {
        void Initialize();
        Room Get(RoomType type);
        void Upgrade(Room room);
    }
    
    public class RoomsService : IRoomsService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IRoomFactory _roomFactory;
        private readonly IRoomSelectionService _roomSelection;
        private readonly IResourceService _resourceService;
        private readonly Dictionary<RoomType, Room> _rooms = new();
        
        public RoomsService(
            IProfileService profile,
            IConfigs configs,
            IRoomFactory roomFactory,
            IRoomSelectionService roomSelection,
            IResourceService resourceService)
        {
            _profile = profile;
            _configs = configs;
            _roomFactory = roomFactory;
            _roomSelection = roomSelection;
            _resourceService = resourceService;
        }

        public void Initialize()
        {
            _roomSelection.Clear();
            _rooms.Clear();

            var tavern = UnityEngine.Object.FindAnyObjectByType<TavernHierarchy>(FindObjectsInactive.Include);
            foreach (var roomContainer in tavern.Rooms)
            {
                roomContainer.Container.ClearChildren();
                var profileData = _profile.Data.Rooms.Find(e => e.Type == roomContainer.Type);
                var configData = _configs.Rooms.GetAll().Find(e => e.Type == roomContainer.Type);
                var roomModel = _roomFactory.Create(profileData.Type, profileData.Level, profileData.Production);
                _rooms.Add(roomModel.Type, roomModel);
                _roomSelection.Register(roomContainer.Collider, roomModel);

                var roomGO = GameObject.Instantiate(configData.Prefab, roomContainer.Container);
                roomGO.transform.localPosition = Vector3.zero;
            }
        }

        public void Upgrade(Room room)
        {
            var roomConfigs = _configs.Rooms.GetAll()
                .Where(data => data.Type == room.Type)
                .ToList();
            var configData = roomConfigs.Find(data => data.Level == room.Level.Value);
            var hasNextLevel = roomConfigs.Any(data => data.Level == room.Level.Value + 1);
            if (configData == null || !hasNextLevel) return;

            var resourceCosts = configData.ResRequirements
                .GroupBy(data => data.Resource)
                .Select(group => new
                {
                    Type = group.Key,
                    Amount = Mathf.CeilToInt(group.Sum(data => data.Count))
                })
                .ToList();

            if (resourceCosts.Any(cost =>
                    cost.Type == ResourceType.None
                    || _resourceService.GetAmount(cost.Type).Value < cost.Amount))
                return;

            if (configData.RoomRequirements.Any(requirement =>
                    !_rooms.TryGetValue(requirement.Type, out var requiredRoom)
                    || requiredRoom.Level.Value < requirement.Level))
                return;

            foreach (var cost in resourceCosts)
                _resourceService.Spend(cost.Type, cost.Amount);

            foreach (var upgrade in configData.ProductionUpgrade.GroupBy(data => data.Type))
            {
                if (_rooms.TryGetValue(upgrade.Key, out var targetRoom))
                    targetRoom.AddProduction(upgrade.Sum(data => data.Count));
            }

            room.UpgradeLevel();
        }

        public Room Get(RoomType type)
        {
            _rooms.TryGetValue(type, out var room);
            return room;
        }
    }
}
