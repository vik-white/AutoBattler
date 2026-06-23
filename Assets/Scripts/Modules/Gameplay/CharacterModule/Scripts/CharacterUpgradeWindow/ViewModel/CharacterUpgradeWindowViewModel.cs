using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterUpgradeWindowViewModel : WindowViewModel<Character>
    {
        private readonly IConfigs _configs;
        private readonly ReactiveProperty<int> _selectedLevel;
        private readonly ReactiveProperty<int> _selectedStars;
        private readonly ReactiveProperty<bool> _canSelectPreviousLevel = new();
        private readonly ReactiveProperty<bool> _canSelectNextLevel = new();

        public string Name;
        public Sprite Image;
        public Sprite ClassIcon;
        public IReadOnlyReactiveProperty<int> SelectedLevel => _selectedLevel;
        public IReadOnlyReactiveProperty<bool> CanSelectPreviousLevel => _canSelectPreviousLevel;
        public IReadOnlyReactiveProperty<bool> CanSelectNextLevel => _canSelectNextLevel;
        public StarsViewModel Stars { get; }
        public StatsInfoViewModel StatsInfo { get; }
        public UnityAction OnSelectPreviousLevel;
        public UnityAction OnSelectNextLevel;

        public CharacterUpgradeWindowViewModel(Character character, IConfigs configs) : base(character)
        {
            _configs = configs;
            Name = character.Config.Name;
            Image = character.Config.Image;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            _selectedLevel = new ReactiveProperty<int>(GetInitialSelectedLevel(character));
            _selectedStars = new ReactiveProperty<int>(character.Stars.Value);
            AddDisposables(_selectedLevel, _selectedStars, _canSelectPreviousLevel, _canSelectNextLevel);
            AddDisposable(character.Level.Subscribe(_ => ClampSelectedLevel()));
            AddDisposable(character.Stars.Subscribe(UpdateSelectedStars));

            Stars = CreateViewModel<StarsViewModel, IReadOnlyReactiveProperty<int>>(_selectedStars);
            StatsInfo = CreateViewModel<StatsInfoViewModel, StatsInfoModel>(
                new StatsInfoModel(character, character.Level, character.Stars, _selectedLevel, _selectedStars));
            OnSelectPreviousLevel = SelectPreviousLevel;
            OnSelectNextLevel = SelectNextLevel;
            RefreshLevelSelectionState();
        }

        private int GetInitialSelectedLevel(Character character)
        {
            var currentLevel = character.Level.Value;
            return Mathf.Clamp(currentLevel + 1, currentLevel, GetMaxSelectableLevel(currentLevel, character.Stars.Value));
        }

        private void SelectPreviousLevel()
        {
            if (_selectedLevel.Value <= Model.Level.Value) return;
            _selectedLevel.Value--;
            RefreshLevelSelectionState();
        }

        private void SelectNextLevel()
        {
            if (_selectedLevel.Value >= GetMaxSelectableLevel()) return;
            _selectedLevel.Value++;
            RefreshLevelSelectionState();
        }

        private void UpdateSelectedStars(int stars)
        {
            _selectedStars.Value = stars;
            ClampSelectedLevel();
        }

        private void ClampSelectedLevel()
        {
            _selectedLevel.Value = Mathf.Clamp(_selectedLevel.Value, Model.Level.Value, GetMaxSelectableLevel());
            RefreshLevelSelectionState();
        }

        private int GetMaxSelectableLevel() => GetMaxSelectableLevel(Model.Level.Value, _selectedStars.Value);

        private int GetMaxSelectableLevel(int currentLevel, int stars) => Mathf.Max(currentLevel, GetMaxLevel(stars));

        private int GetMaxLevel(int stars) => _configs.Stars.Get(Mathf.Max(0, stars - 1)).Level;

        private void RefreshLevelSelectionState()
        {
            _canSelectPreviousLevel.Value = _selectedLevel.Value > Model.Level.Value;
            _canSelectNextLevel.Value = _selectedLevel.Value < GetMaxSelectableLevel();
        }

        public override void Dispose()
        {
            base.Dispose();
            OnSelectPreviousLevel = null;
            OnSelectNextLevel = null;
        }
    }
}
