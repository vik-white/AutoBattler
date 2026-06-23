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
        public IReadOnlyList<SkillItemViewModel> Skills => _skills;
        public IReadOnlyReactiveProperty<string> SkillName => _skillName;
        public IReadOnlyReactiveProperty<string> SkillDescription => _skillDescription;
        public IReadOnlyReactiveProperty<string> SkillUpgradePrice => _skillUpgradePrice;
        public IReadOnlyReactiveProperty<bool> CanUpgradeSkill => _canUpgradeSkill;
        public IReadOnlyReactiveProperty<int> BooksAmount { get; }
        public IReadOnlyReactiveProperty<int> ClassBooksAmount { get; }
        public UnityAction OnOpenStats;
        public UnityAction OnUpgradeSkill;

        public CharacterSkillsWindowViewModel(Character character, IConfigs configs, IResourceService resources, ICharacterWindow characterWindow) : base(character)
        {
            _configs = configs;
            _resources = resources;
            _characterWindow = characterWindow;
            _classBookResource = ResourceHandler.GetBookResourceType(character.Config.Class);

            Name = character.Config.Name;
            Image = character.Config.Image;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            BookClassIcon = configs.UI.ResourceIcons[_classBookResource];
            BooksAmount = resources.GetAmount(ResourceType.Book);
            ClassBooksAmount = resources.GetAmount(_classBookResource);
            OnOpenStats = OpenStats;
            OnUpgradeSkill = UpgradeSkill;

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
            Model.UpgradeSkill(_selectedSkill.Value.Slot);
        }

        private void OpenStats()
        {
            _characterWindow.ShowWindow(Model);
            Close();
        }

        public override void Dispose()
        {
            base.Dispose();
            OnOpenStats = null;
            OnUpgradeSkill = null;
        }
    }
}
