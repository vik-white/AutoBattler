using System.Collections.Generic;
using UniRx;
using UnityEngine;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public class StatsInfoViewModel : ViewModel<StatsInfoModel>
    {
        private readonly List<StatViewModel> _stats = new();

        public IReadOnlyList<StatViewModel> Stats => _stats;

        public StatsInfoViewModel(StatsInfoModel model) : base(model)
        {
            CreateStats();
            AddDisposable(model.CurrentLevel.Subscribe(_ => RefreshStats()));
            AddDisposable(model.CurrentStars.Subscribe(_ => RefreshStats()));
            AddDisposable(model.SelectedLevel.Subscribe(_ => RefreshStats()));
            AddDisposable(model.SelectedStars.Subscribe(_ => RefreshStats()));
            RefreshStats();
        }

        private void CreateStats()
        {
            _stats.Add(CreateViewModel<StatViewModel, StatInfoModel>(new StatInfoModel("Damage", StatType.Attack)));
            _stats.Add(CreateViewModel<StatViewModel, StatInfoModel>(new StatInfoModel("Defense", StatType.Defense)));
            _stats.Add(CreateViewModel<StatViewModel, StatInfoModel>(new StatInfoModel("Health", StatType.Health)));
            _stats.Add(CreateViewModel<StatViewModel, StatInfoModel>(new StatInfoModel("Crit Damage", StatType.CritValue)));
            _stats.Add(CreateViewModel<StatViewModel, StatInfoModel>(new StatInfoModel("Crit Chance", StatType.CritChance)));
        }

        private void RefreshStats()
        {
            for (int i = 0; i < _stats.Count; i++)
            {
                var stat = _stats[i];
                var currentValue = CharacterStatsHandler.Calculate(Model.Character, stat.Type);
                var selectedValue = CharacterStatsHandler.Calculate(Model.Character, stat.Type, Model.SelectedLevel.Value, Model.SelectedStars.Value);
                stat.UpdateValue(currentValue, selectedValue);
            }
        }
    }
}
