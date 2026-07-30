using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public class BreakthroughWindowViewModel : WindowViewModel<Character>
    {
        private readonly IConfigs _configs;
        private readonly IResourceService _resources;
        private readonly IBreakthroughService _breakthroughService;
        private readonly ReactiveProperty<string> _currentLevel = new();
        private readonly ReactiveProperty<string> _nextLevel = new();
        private readonly ReactiveProperty<string> _attackCurrent = new();
        private readonly ReactiveProperty<string> _attackAdd = new();
        private readonly ReactiveProperty<string> _defenseCurrent = new();
        private readonly ReactiveProperty<string> _defenseAdd = new();
        private readonly ReactiveProperty<string> _healthCurrent = new();
        private readonly ReactiveProperty<string> _healthAdd = new();
        private readonly ReactiveProperty<string> _essenceCount = new();
        private readonly ReactiveProperty<string> _expCount = new();
        private readonly ReactiveProperty<string> _heroesDesc = new();
        private readonly ReactiveProperty<string> _heroesCount = new();
        private readonly ReactiveProperty<bool> _canBreakthrough = new();

        public IReadOnlyReactiveProperty<string> CurrentLevel => _currentLevel;
        public IReadOnlyReactiveProperty<string> NextLevel => _nextLevel;
        public IReadOnlyReactiveProperty<string> AttackCurrent => _attackCurrent;
        public IReadOnlyReactiveProperty<string> AttackAdd => _attackAdd;
        public IReadOnlyReactiveProperty<string> DefenseCurrent => _defenseCurrent;
        public IReadOnlyReactiveProperty<string> DefenseAdd => _defenseAdd;
        public IReadOnlyReactiveProperty<string> HealthCurrent => _healthCurrent;
        public IReadOnlyReactiveProperty<string> HealthAdd => _healthAdd;
        public IReadOnlyReactiveProperty<string> EssenceCount => _essenceCount;
        public IReadOnlyReactiveProperty<string> ExpCount => _expCount;
        public IReadOnlyReactiveProperty<string> HeroesDesc => _heroesDesc;
        public IReadOnlyReactiveProperty<string> HeroesCount => _heroesCount;
        public IReadOnlyReactiveProperty<bool> CanBreakthrough => _canBreakthrough;
        public UnityAction OnBreakthrough;

        public BreakthroughWindowViewModel(
            Character character,
            IConfigs configs,
            IResourceService resources,
            ICharactersService characters,
            IBreakthroughService breakthroughService) : base(character)
        {
            _configs = configs;
            _resources = resources;
            _breakthroughService = breakthroughService;

            AddDisposables(
                _currentLevel,
                _nextLevel,
                _attackCurrent,
                _attackAdd,
                _defenseCurrent,
                _defenseAdd,
                _healthCurrent,
                _healthAdd,
                _essenceCount,
                _expCount,
                _heroesDesc,
                _heroesCount,
                _canBreakthrough);

            AddDisposable(character.Level.Subscribe(_ => Refresh()));
            AddDisposable(resources.GetAmount(ResourceType.Essence).Subscribe(_ => RefreshRequirements()));
            AddDisposable(resources.GetAmount(ResourceType.Exp).Subscribe(_ => RefreshRequirements()));
            foreach (var hero in characters.GetCharacters())
                AddDisposable(hero.Level.Subscribe(_ => RefreshRequirements()));

            OnBreakthrough = Breakthrough;
            Refresh();
        }

        private void Refresh()
        {
            var currentLevel = Model.Level.Value;
            var nextLevel = currentLevel + 1;
            _currentLevel.Value = $"Lv. {currentLevel}";
            _nextLevel.Value = $"Lv. {nextLevel}";
            _heroesDesc.Value = $"Level up heroes to {currentLevel} level:";

            RefreshStat(StatType.Attack, currentLevel, nextLevel, _attackCurrent, _attackAdd);
            RefreshStat(StatType.Defense, currentLevel, nextLevel, _defenseCurrent, _defenseAdd);
            RefreshStat(StatType.Health, currentLevel, nextLevel, _healthCurrent, _healthAdd);
            RefreshRequirements();
        }

        private void RefreshStat(
            StatType stat,
            int currentLevel,
            int nextLevel,
            ReactiveProperty<string> currentText,
            ReactiveProperty<string> addText)
        {
            var current = CharacterStatsHandler.Calculate(Model, stat, currentLevel, Model.Stars.Value);
            var next = CharacterStatsHandler.Calculate(Model, stat, nextLevel, Model.Stars.Value);
            currentText.Value = FormatNumber(current);
            addText.Value = FormatAdd(next - current);
        }

        private void RefreshRequirements()
        {
            var settings = _configs.Settings;
            var essence = _resources.GetAmount(ResourceType.Essence).Value;
            var exp = _resources.GetAmount(ResourceType.Exp).Value;
            var heroes = _breakthroughService.GetEligibleHeroesCount(Model);

            _essenceCount.Value = FormatRequirement(essence, settings.BreakthroughEssence);
            _expCount.Value = FormatRequirement(exp, settings.BreakthroughExp);
            _heroesCount.Value = FormatRequirement(heroes, settings.BreakthroughHeroesCount);
            _canBreakthrough.Value = _breakthroughService.CanBreakthrough(Model);
        }

        private void Breakthrough()
        {
            if (!_breakthroughService.TryBreakthrough(Model))
            {
                RefreshRequirements();
                return;
            }

            Close();
        }

        private static string FormatRequirement(int current, int required)
        {
            var color = current >= required ? ColorHandler.Green : ColorHandler.Red;
            return $"{current.ToString().Color(color)}/{required}";
        }

        private static string FormatAdd(float value)
        {
            var prefix = value >= 0 ? "+" : "";
            return $"{prefix}{FormatNumber(value)}";
        }

        private static string FormatNumber(float value)
        {
            var rounded = Mathf.Round(value);
            return Mathf.Approximately(value, rounded)
                ? Mathf.RoundToInt(value).ToString()
                : $"{value:0.#}";
        }

        public override void Dispose()
        {
            base.Dispose();
            OnBreakthrough = null;
        }
    }
}
