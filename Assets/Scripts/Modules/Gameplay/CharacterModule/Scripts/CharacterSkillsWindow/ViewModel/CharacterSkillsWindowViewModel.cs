using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterSkillsWindowViewModel : WindowViewModel<Character>
    {
        private readonly IConfigs _configs;
        private readonly IResourceService _resources;
        private readonly ICharacterWindow _characterWindow;
        private readonly ICharacterSkillsWindow _characterSkillsWindow;
        private readonly ResourceType _classBookResource;
        private readonly List<SkillItemViewModel> _skills = new();
        private readonly ReactiveProperty<SkillItemViewModel> _selectedSkill = new();
        private readonly ReactiveProperty<string> _skillName = new();
        private readonly ReactiveProperty<string> _skillDescription = new();
        private readonly ReactiveProperty<string> _skillUpgradePrice = new();
        private readonly ReactiveProperty<bool> _canUpgradeSkill = new();

        public string Name { get; }
        public Sprite Image { get; }
        public Sprite ClassIcon { get; }
        public Sprite BookClassIcon { get; }
        private readonly List<Character> _characters;
        public IReadOnlyList<SkillItemViewModel> Skills => _skills;
        public IReadOnlyReactiveProperty<string> SkillName => _skillName;
        public IReadOnlyReactiveProperty<string> SkillDescription => _skillDescription;
        public IReadOnlyReactiveProperty<string> SkillUpgradePrice => _skillUpgradePrice;
        public IReadOnlyReactiveProperty<bool> CanUpgradeSkill => _canUpgradeSkill;
        public IReadOnlyReactiveProperty<int> BooksAmount { get; }
        public IReadOnlyReactiveProperty<int> ClassBooksAmount { get; }
        public UnityAction OnSelectPreviousCharacter;
        public UnityAction OnSelectNextCharacter;
        public UnityAction OnOpenStats;
        public UnityAction OnUpgradeSkill;
        public UnityAction OnRedeem;

        public CharacterSkillsWindowViewModel(
            Character character, 
            IConfigs configs, 
            IResourceService resources, 
            ICharacterWindow characterWindow, 
            ICharacterSkillsWindow characterSkillsWindow,
            IRedeemBookWindow redeemBookWindow,
            ICharactersService charactersService) : base(character)
        {
            _configs = configs;
            _resources = resources;
            _characterWindow = characterWindow;
            _characterSkillsWindow = characterSkillsWindow;
            _classBookResource = ResourceHandler.GetBookResourceType(character.Config.Class);
            _characters = new List<Character>(charactersService.GetCharacters());

            Name = character.Config.Name;
            Image = character.Config.Image;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            BookClassIcon = configs.UI.ResourceIcons[_classBookResource];
            BooksAmount = resources.GetAmount(ResourceType.Book);
            ClassBooksAmount = resources.GetAmount(_classBookResource);
            OnOpenStats = OpenStats;
            OnUpgradeSkill = UpgradeSkill;
            OnRedeem = () => redeemBookWindow.ShowWindow(character);
            OnSelectPreviousCharacter = SelectPreviousCharacter;
            OnSelectNextCharacter = SelectNextCharacter;

            AddDisposables(_selectedSkill, _skillName, _skillDescription, _skillUpgradePrice, _canUpgradeSkill);
            CreateSkills();
            SelectSkill(_skills[0]);
            foreach (var skill in character.Skills) AddDisposable(skill.Level.Subscribe(_ => RefreshSkills()));
            AddDisposable(character.Stars.Subscribe(_ => RefreshSkills()));
            AddDisposable(ClassBooksAmount.Subscribe(_ => RefreshUpgradeState()));
            RefreshSkills();
        }

        private void CreateSkills()
        {
            foreach (var slot in SkillSlotExtensions.UpgradableSlots)
            {
                var skill = Model.GetSkill(slot);
                if (skill == null) continue;
                var viewModel = CreateViewModel<SkillItemViewModel, CharacterSkill>(skill);
                viewModel.OnSelect = () => SelectSkill(viewModel);
                _skills.Add(viewModel);
            }
        }

        private void SelectSkill(SkillItemViewModel skill)
        {
            _selectedSkill.Value = skill;
            foreach (var item in _skills) item.SetSelected(item == skill);
            RefreshSelectedSkill();
        }

        private void RefreshSkills()
        {
            foreach (var skill in _skills) skill.SetLevel(GetSkillLevel(skill.Slot));
            RefreshSelectedSkill();
        }

        private void RefreshSelectedSkill()
        {
            _skillName.Value = $"{_selectedSkill.Value.Name} Lv.{_selectedSkill.Value.Level.Value}";
            _skillDescription.Value = _selectedSkill.Value.Description;
            RefreshUpgradeState();
        }

        private void RefreshUpgradeState()
        {
            var price = _configs.Settings.SkillUpPrice;
            _skillUpgradePrice.Value = $"{ClassBooksAmount.Value}/{price}";
            var selected = _selectedSkill.Value;
            var maxLevel = selected == null ? 0 : Model.GetMaxSkillLevel(selected.Slot);
            var skillLevel = selected == null ? 0 : Model.GetSkillLevel(selected.Slot);
            _canUpgradeSkill.Value = selected != null && selected.IsVisible.Value && maxLevel > 0 && skillLevel < maxLevel && (price <= 0 || ClassBooksAmount.Value >= price);
        }
        
        private int GetSkillLevel(SkillSlotType slot)
        {
            var maxLevel = Model.GetMaxSkillLevel(slot);
            return maxLevel <= 0 ? 0 : Mathf.Min(Model.GetSkillLevel(slot), maxLevel);
        }

        private void UpgradeSkill()
        {
            var maxLevel = Model.GetMaxSkillLevel(_selectedSkill.Value.Slot);
            if (maxLevel <= 0 || Model.GetSkillLevel(_selectedSkill.Value.Slot) >= maxLevel) return;
            var price = _configs.Settings.SkillUpPrice;
            if (price > 0 && ClassBooksAmount.Value < price) return;
            if (price > 0) _resources.Spend(_classBookResource, price);
            Model.UpgradeSkill(_selectedSkill.Value.ID);
        }

        private void OpenStats()
        {
            _characterWindow.ShowWindow(Model);
            Close();
        }
        
        private void SelectPreviousCharacter() => SelectCharacter(-1);

        private void SelectNextCharacter() => SelectCharacter(1);

        private void SelectCharacter(int direction)
        {
            if (_characters.Count <= 1) return;

            var index = _characters.FindIndex(character => character.ID == Model.ID);
            if (index < 0) return;

            var nextIndex = (index + direction + _characters.Count) % _characters.Count;
            _characterSkillsWindow.ShowWindow(_characters[nextIndex]);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnOpenStats = null;
            OnUpgradeSkill = null;
            OnRedeem = null;
            OnSelectPreviousCharacter = null;
            OnSelectNextCharacter = null;
        }
    }
}
