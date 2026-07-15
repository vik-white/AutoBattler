using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class RoomWindowContent
    {
        public IReadOnlyList<RoomLineModel> Production { get; }
        public IReadOnlyList<RoomLineModel> Requirements { get; }
        public IReadOnlyList<RoomLineModel> Upgrades { get; }

        public RoomWindowContent(
            IReadOnlyList<RoomLineModel> production,
            IReadOnlyList<RoomLineModel> requirements,
            IReadOnlyList<RoomLineModel> upgrades)
        {
            Production = production;
            Requirements = requirements;
            Upgrades = upgrades;
        }
    }

    public class RoomWindowViewModel : WindowViewModel<Room>
    {
        private readonly IConfigs _configs;
        private readonly IResourceService _resourceService;
        private readonly IRoomsService _roomsService;
        private readonly CompositeDisposable _requirementSubscriptions = new();
        private readonly ReactiveProperty<RoomWindowContent> _content = new();
        private readonly ReactiveProperty<bool> _canUpgrade = new();

        public string Title => Model.Type.ToString();
        public IReadOnlyReactiveProperty<int> Level => Model.Level;
        public IReadOnlyReactiveProperty<RoomWindowContent> Content => _content;
        public IReadOnlyReactiveProperty<bool> CanUpgrade => _canUpgrade;
        public UnityAction OnUpgrade;

        public RoomWindowViewModel(
            Room room,
            IConfigs configs,
            IResourceService resourceService,
            IRoomsService roomsService) : base(room)
        {
            _configs = configs;
            _resourceService = resourceService;
            _roomsService = roomsService;
            AddDisposables(_content, _canUpgrade, _requirementSubscriptions);
            AddDisposable(Level
                .CombineLatest(Model.Production, (level, _) => level)
                .Subscribe(RefreshContent));
            OnUpgrade = Upgrade;
        }

        private void RefreshContent(int level)
        {
            var roomConfigs = _configs.Rooms.GetAll()
                .Where(data => data.Type == Model.Type)
                .OrderBy(data => data.Level)
                .ToList();

            var currentConfig = roomConfigs.Find(data => data.Level == level);
            var productionConfig = roomConfigs.LastOrDefault(data => data.Level <= level);

            _content.Value = new RoomWindowContent(
                CreateProductionLines(productionConfig, Model.Production.Value),
                CreateRequirementLines(currentConfig),
                CreateUpgradeLines(currentConfig));
            BindRequirementSubscriptions(currentConfig);
        }

        private void BindRequirementSubscriptions(IRoomData currentConfig)
        {
            _requirementSubscriptions.Clear();
            if (currentConfig == null)
            {
                _canUpgrade.Value = false;
                return;
            }

            foreach (var resource in currentConfig.ResRequirements
                         .Select(data => data.Resource)
                         .Where(type => type != ResourceType.None)
                         .Distinct())
            {
                _resourceService.GetAmount(resource)
                    .Subscribe(_ => RefreshCanUpgrade(currentConfig))
                    .AddTo(_requirementSubscriptions);
            }

            foreach (var roomType in currentConfig.RoomRequirements
                         .Select(data => data.Type)
                         .Distinct())
            {
                var requiredRoom = _roomsService.Get(roomType);
                if (requiredRoom == null) continue;

                requiredRoom.Level
                    .Subscribe(_ => RefreshCanUpgrade(currentConfig))
                    .AddTo(_requirementSubscriptions);
            }

            RefreshCanUpgrade(currentConfig);
        }

        private void RefreshCanUpgrade(IRoomData currentConfig)
        {
            _canUpgrade.Value = currentConfig != null
                                && currentConfig.ResRequirements.All(IsResourceRequirementMet)
                                && currentConfig.RoomRequirements.All(IsRoomRequirementMet);
        }

        private bool IsResourceRequirementMet(ResourceCountData requirement)
        {
            return requirement.Resource != ResourceType.None
                   && _resourceService.GetAmount(requirement.Resource).Value >= requirement.Count;
        }

        private bool IsRoomRequirementMet(RoomLevelData requirement)
        {
            var room = _roomsService.Get(requirement.Type);
            return room != null && room.Level.Value >= requirement.Level;
        }

        private static IReadOnlyList<RoomLineModel> CreateProductionLines(
            IRoomData currentConfig,
            float production)
        {
            if (currentConfig == null || currentConfig.Production == ResourceType.None)
                return Array.Empty<RoomLineModel>();

            return new[]
            {
                new RoomLineModel(currentConfig.Production.ToString(), FormatNumber(production))
            };
        }

        private static IReadOnlyList<RoomLineModel> CreateRequirementLines(IRoomData currentConfig)
        {
            if (currentConfig == null) return Array.Empty<RoomLineModel>();

            var lines = new List<RoomLineModel>();
            foreach (var requirement in currentConfig.ResRequirements)
                lines.Add(new RoomLineModel(requirement.Resource.ToString(), FormatNumber(requirement.Count)));

            foreach (var requirement in currentConfig.RoomRequirements)
                lines.Add(new RoomLineModel(requirement.Type.ToString(), $"Lv.{requirement.Level}"));

            return lines;
        }

        private static IReadOnlyList<RoomLineModel> CreateUpgradeLines(IRoomData currentConfig)
        {
            if (currentConfig == null) return Array.Empty<RoomLineModel>();

            var lines = new List<RoomLineModel>();
            foreach (var upgrade in currentConfig.ProductionUpgrade)
                lines.Add(new RoomLineModel($"{upgrade.Type} Production", FormatUpgrade(upgrade.Count)));

            foreach (var upgrade in currentConfig.CapacityUpgrade)
                lines.Add(new RoomLineModel($"{upgrade.Resource} Capacity", FormatUpgrade(upgrade.Count)));

            return lines;
        }

        private void Upgrade()
        {
            if (_canUpgrade.Value) Model.Upgrade();
        }

        private static string FormatUpgrade(float value)
        {
            var prefix = value >= 0 ? "+" : "";
            return $"{prefix}{FormatNumber(value)}";
        }

        private static string FormatNumber(float value)
        {
            var rounded = Mathf.Round(value);
            return Mathf.Approximately(value, rounded) ? Mathf.RoundToInt(value).ToString() : $"{value:0.#}";
        }

        public override void Dispose()
        {
            base.Dispose();
            OnUpgrade = null;
        }
    }
}
