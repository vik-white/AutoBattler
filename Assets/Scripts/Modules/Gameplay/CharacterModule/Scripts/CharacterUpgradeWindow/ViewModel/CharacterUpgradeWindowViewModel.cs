using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterUpgradeWindowViewModel : WindowViewModel<Character>
    {
        private readonly IResourceService _resource;
        private readonly ReactiveProperty<int> _selectedLevel;
        private readonly ReactiveProperty<int> _selectedStars;
        private readonly ReactiveProperty<bool> _canSelectPreviousLevel = new();
        private readonly ReactiveProperty<bool> _canSelectNextLevel = new();
        private readonly ReadOnlyReactiveProperty<string> _might;

        public string Name;
        public Sprite Image;
        public Sprite ClassIcon;
        public IReadOnlyReactiveProperty<int> SelectedLevel => _selectedLevel;
        public IReadOnlyReactiveProperty<bool> CanSelectPreviousLevel => _canSelectPreviousLevel;
        public IReadOnlyReactiveProperty<bool> CanSelectNextLevel => _canSelectNextLevel;
        public IReadOnlyReactiveProperty<string> Might => _might;
        public ResourceViewModel ExpResources;
        public int LevelUpPrice;
        public StarsViewModel Stars { get; }
        public StatsInfoViewModel StatsInfo { get; }
        public UnityAction OnUpgradeLevel;
        public UnityAction OnSelectPreviousLevel;
        public UnityAction OnSelectNextLevel;

        public CharacterUpgradeWindowViewModel(Character character, IConfigs configs, IResourceService resource) : base(character)
        {
            _resource = resource;
            Name = character.Config.Name;
            Image = character.Config.Image;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            LevelUpPrice = configs.Settings.LevelUpPrice;
            _selectedLevel = new ReactiveProperty<int>(GetInitialSelectedLevel(character));
            _selectedStars = new ReactiveProperty<int>(character.Stars.Value);
            _might = character.Might.CombineLatest(_selectedLevel, GetMightText).ToReadOnlyReactiveProperty();
            AddDisposables(_selectedLevel, _selectedStars, _canSelectPreviousLevel, _canSelectNextLevel, _might);
            AddDisposable(character.Level.Subscribe(_ => ClampSelectedLevel()));
            AddDisposable(character.Stars.Subscribe(UpdateSelectedStars));

            ExpResources = CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Exp));
            Stars = CreateViewModel<StarsViewModel, StarsModel>(new StarsModel(_selectedStars));
            StatsInfo = CreateViewModel<StatsInfoViewModel, StatsInfoModel>(
                new StatsInfoModel(character, character.Level, character.Stars, _selectedLevel, _selectedStars));
            OnUpgradeLevel = LevelUpgrade;
            OnSelectPreviousLevel = SelectPreviousLevel;
            OnSelectNextLevel = SelectNextLevel;
            RefreshLevelSelectionState();
        }

        private string GetMightText(int currentMight, int selectedLevel)
        {
            var selectedLevelMight = MightHandler.Calculate(Model, selectedLevel, Model.Stars.Value);
            return $"{currentMight} <color=#B2DE2A>+{selectedLevelMight - currentMight}</color>";
        }

        private void LevelUpgrade()
        {
            if (Model.GetMaxLevel() <= Model.Level.Value) return;
            if (_resource.GetAmount(ResourceType.Exp).Value < LevelUpPrice) return;
            _resource.Spend(ResourceType.Exp, LevelUpPrice);
            Model.UpgradeLevel();
        }

        private int GetInitialSelectedLevel(Character character)
        {
            var currentLevel = character.Level.Value;
            return currentLevel + 1;
        }

        private void SelectPreviousLevel()
        {
            if (_selectedLevel.Value <= Model.Level.Value) return;
            _selectedLevel.Value--;
            RefreshLevelSelectionState();
        }

        private void SelectNextLevel()
        {
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
            _selectedLevel.Value = Mathf.Max(_selectedLevel.Value, Model.Level.Value);
            RefreshLevelSelectionState();
        }

        private void RefreshLevelSelectionState()
        {
            _canSelectPreviousLevel.Value = _selectedLevel.Value > Model.Level.Value;
            _canSelectNextLevel.Value = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnUpgradeLevel = null;
            OnSelectPreviousLevel = null;
            OnSelectNextLevel = null;
        }
    }
}
