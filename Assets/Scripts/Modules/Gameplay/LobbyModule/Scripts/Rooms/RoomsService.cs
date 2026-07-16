using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public readonly struct RoomUpgradeState
    {
        public bool IsUpgrading { get; }
        public float Progress { get; }
        public long SecondsRemaining { get; }

        public RoomUpgradeState(bool isUpgrading, float progress, long secondsRemaining)
        {
            IsUpgrading = isUpgrading;
            Progress = progress;
            SecondsRemaining = secondsRemaining;
        }
    }

    public interface IRoomsService
    {
        void Initialize();
        void Clear();
        Room Get(RoomType type);
        RoomUpgradeState GetUpgradeState(Room room);
        void Upgrade(Room room);
        void CollectProduction(Room room, ResourceType resourceType);
    }
    
    public class RoomsService : IRoomsService
    {
        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IRoomFactory _roomFactory;
        private readonly IRoomSelectionService _roomSelection;
        private readonly IResourceService _resourceService;
        private readonly IRoomProductionViewFactory _roomProductionFactory;
        private readonly IRoomProgressViewFactory _roomProgressFactory;
        private readonly IUIRoot _uiRoot;
        private readonly Dictionary<RoomType, Room> _rooms = new();
        private readonly Dictionary<RoomType, Transform> _roomSceneContainers = new();
        private readonly List<RoomProductionView> _roomProductionViews = new();
        private readonly List<RoomProgressView> _roomProgressViews = new();
        private readonly CompositeDisposable _upgradeSubscriptions = new();
        
        public RoomsService(
            IProfileService profile,
            IConfigs configs,
            IRoomFactory roomFactory,
            IRoomSelectionService roomSelection,
            IResourceService resourceService,
            IRoomProductionViewFactory roomProductionFactory,
            IRoomProgressViewFactory roomProgressFactory,
            IUIRoot uiRoot)
        {
            _profile = profile;
            _configs = configs;
            _roomFactory = roomFactory;
            _roomSelection = roomSelection;
            _resourceService = resourceService;
            _roomProductionFactory = roomProductionFactory;
            _roomProgressFactory = roomProgressFactory;
            _uiRoot = uiRoot;
        }

        public void Initialize()
        {
            Clear();

            var tavern = UnityEngine.Object.FindAnyObjectByType<TavernHierarchy>(FindObjectsInactive.Include);
            foreach (var roomContainer in tavern.Rooms)
            {
                var profileData = _profile.Data.Rooms.Find(e => e.Type == roomContainer.Type);
                var roomConfigs = _configs.Rooms.GetAll()
                    .Where(data => data.Type == roomContainer.Type)
                    .OrderBy(data => data.Level)
                    .ToList();
                var roomModel = _roomFactory.Create(
                    profileData.Type,
                    profileData.Level,
                    profileData.Production,
                    profileData.Capacity,
                    profileData.LastProductionCollectionUnixTime,
                    profileData.UpgradeStartUnixTime);
                _rooms.Add(roomModel.Type, roomModel);
                _roomSceneContainers.Add(roomModel.Type, roomContainer.Container);
                _roomSelection.Register(roomContainer.Collider, roomModel);
                ReplaceRoomPrefab(roomModel);

                var productionConfig = roomConfigs.LastOrDefault(data => data.Level <= roomModel.Level.Value);
                if (productionConfig != null && productionConfig.Production != ResourceType.None)
                {
                    var productionModel = new RoomProductionModel(
                        roomModel,
                        productionConfig.Production,
                        roomContainer.Collider);
                    _roomProductionViews.Add(_roomProductionFactory.Get(
                        productionModel,
                        _uiRoot.GetLayer(UILayer.WORLD)));
                }

                var progressModel = new RoomProgressModel(roomModel, roomContainer.Collider);
                _roomProgressViews.Add(_roomProgressFactory.Get(
                    progressModel,
                    _uiRoot.GetLayer(UILayer.WORLD)));
            }

            CompleteFinishedUpgrades();
            Observable.Interval(TimeSpan.FromSeconds(1), Scheduler.MainThreadIgnoreTimeScale)
                .Subscribe(_ => CompleteFinishedUpgrades())
                .AddTo(_upgradeSubscriptions);
        }

        public void Clear()
        {
            _upgradeSubscriptions.Clear();

            for (var i = 0; i < _roomProductionViews.Count; i++)
                _roomProductionViews[i].DisposeAndDestroy();

            for (var i = 0; i < _roomProgressViews.Count; i++)
                _roomProgressViews[i].DisposeAndDestroy();

            _roomProductionViews.Clear();
            _roomProgressViews.Clear();
            _roomSelection.Clear();
            _roomSceneContainers.Clear();
            _rooms.Clear();
        }

        public void Upgrade(Room room)
        {
            if (room == null || room.IsUpgrading) return;

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

            var upgradeStartUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            room.SetLastProductionCollectionTime(upgradeStartUnixTime);
            room.SetUpgradeStartTime(upgradeStartUnixTime);
            CompleteFinishedUpgrades(upgradeStartUnixTime);
        }

        public RoomUpgradeState GetUpgradeState(Room room)
        {
            if (room == null
                || !room.IsUpgrading
                || !TryGetUpgradeData(room, out _, out var completionUnixTime))
                return default;

            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var durationSeconds = completionUnixTime - room.UpgradeStartUnixTime.Value;
            var elapsedSeconds = Math.Max(0, currentUnixTime - room.UpgradeStartUnixTime.Value);
            var progress = durationSeconds <= 0
                ? 1f
                : Mathf.Clamp01((float)elapsedSeconds / durationSeconds);

            return new RoomUpgradeState(
                true,
                progress,
                Math.Max(0, completionUnixTime - currentUnixTime));
        }

        private void CompleteFinishedUpgrades()
        {
            CompleteFinishedUpgrades(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        private void CompleteFinishedUpgrades(long currentUnixTime)
        {
            var completedUpgrades = new List<(Room Room, IRoomData Config, long CompletionUnixTime)>();
            foreach (var room in _rooms.Values)
            {
                if (!room.IsUpgrading) continue;

                if (!TryGetUpgradeData(room, out var config, out var completionUnixTime))
                {
                    room.SetLastProductionCollectionTime(currentUnixTime);
                    room.SetUpgradeStartTime(0);
                    continue;
                }

                if (completionUnixTime <= currentUnixTime)
                    completedUpgrades.Add((room, config, completionUnixTime));
            }

            foreach (var upgradeData in completedUpgrades.OrderBy(data => data.CompletionUnixTime))
                CompleteUpgrade(upgradeData.Room, upgradeData.Config, upgradeData.CompletionUnixTime);
        }

        private bool TryGetUpgradeData(Room room, out IRoomData config, out long completionUnixTime)
        {
            var roomConfigs = _configs.Rooms.GetAll()
                .Where(data => data.Type == room.Type)
                .ToList();
            config = roomConfigs.Find(data => data.Level == room.Level.Value);
            var hasNextLevel = roomConfigs.Any(data => data.Level == room.Level.Value + 1);
            if (config == null || !hasNextLevel)
            {
                completionUnixTime = 0;
                return false;
            }

            var durationSeconds = Math.Max(0L, (long)Math.Round(config.UpgradeTime * 60d * 60d));
            completionUnixTime = room.UpgradeStartUnixTime.Value + durationSeconds;
            return true;
        }

        private void CompleteUpgrade(Room room, IRoomData configData, long completionUnixTime)
        {
            room.SetLastProductionCollectionTime(completionUnixTime);
            foreach (var upgrade in configData.ProductionUpgrade.GroupBy(data => data.Type))
            {
                if (_rooms.TryGetValue(upgrade.Key, out var targetRoom))
                    targetRoom.AddProduction(upgrade.Sum(data => data.Count));
            }

            foreach (var upgrade in configData.CapacityUpgrade.GroupBy(data => data.Type))
            {
                if (_rooms.TryGetValue(upgrade.Key, out var targetRoom))
                    targetRoom.AddCapacity(upgrade.Sum(data => data.Count));
            }

            room.UpgradeLevel();
            ReplaceRoomPrefab(room);
            room.SetUpgradeStartTime(0);
        }

        private void ReplaceRoomPrefab(Room room)
        {
            if (room == null
                || !_roomSceneContainers.TryGetValue(room.Type, out var container))
                return;

            var config = _configs.Rooms.GetAll()
                .FirstOrDefault(data => data.Type == room.Type && data.Level == room.Level.Value);
            if (config?.Prefab == null) return;

            container.ClearChildren();
            var roomGameObject = GameObject.Instantiate(config.Prefab, container);
            roomGameObject.transform.localPosition = Vector3.zero;
        }

        public void CollectProduction(Room room, ResourceType resourceType)
        {
            if (room == null || room.IsUpgrading || resourceType == ResourceType.None) return;

            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var state = RoomProductionCalculator.Calculate(
                room.LastProductionCollectionUnixTime.Value,
                room.Production.Value,
                room.Capacity.Value,
                currentUnixTime);
            var amount = state.CollectibleAmount;

            if (amount <= 0) return;

            _resourceService.Add(resourceType, amount);
            room.SetLastProductionCollectionTime(currentUnixTime);
        }

        public Room Get(RoomType type)
        {
            _rooms.TryGetValue(type, out var room);
            return room;
        }
    }
}
