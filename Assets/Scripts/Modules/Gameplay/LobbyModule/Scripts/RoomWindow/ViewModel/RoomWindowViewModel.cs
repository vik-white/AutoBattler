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
        private readonly ReactiveProperty<RoomWindowContent> _content = new();
        private readonly ReactiveProperty<bool> _canUpgrade = new();

        public string Title => Model.Type.ToString();
        public IReadOnlyReactiveProperty<int> Level => Model.Level;
        public IReadOnlyReactiveProperty<RoomWindowContent> Content => _content;
        public IReadOnlyReactiveProperty<bool> CanUpgrade => _canUpgrade;
        public UnityAction OnUpgrade;

        public RoomWindowViewModel(Room room, IConfigs configs) : base(room)
        {
            _configs = configs;
            AddDisposables(_content, _canUpgrade);
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

            _canUpgrade.Value = currentConfig != null;
            _content.Value = new RoomWindowContent(
                CreateProductionLines(productionConfig, Model.Production.Value),
                CreateRequirementLines(currentConfig),
                CreateUpgradeLines(currentConfig));
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
