using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterSkillsWindowViewModel : WindowViewModel<Character>
    {
        private static readonly SkillSlotType[] SlotOrder =
        {
            SkillSlotType.Active,
            SkillSlotType.Passive1,
            SkillSlotType.Passive2,
            SkillSlotType.Meta1,
            SkillSlotType.Meta2,
            SkillSlotType.Meta3,
        };

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
        public IReadOnlyList<SkillItemViewModel> Skills => _skills;
        public IReadOnlyReactiveProperty<string> SkillName => _skillName;
        public IReadOnlyReactiveProperty<string> SkillDescription => _skillDescription;
        public IReadOnlyReactiveProperty<string> SkillUpgradePrice => _skillUpgradePrice;
        public IReadOnlyReactiveProperty<bool> CanUpgradeSkill => _canUpgradeSkill;
        public IReadOnlyReactiveProperty<int> BooksAmount { get; }
        public IReadOnlyReactiveProperty<int> ClassBooksAmount { get; }
        public UnityAction OnOpenStats;
        public UnityAction OnUpgradeSkill;

        public CharacterSkillsWindowViewModel(
            Character character,
            IConfigs configs,
            IResourceService resources,
            ICharacterWindow characterWindow) : base(character)
        {
            _configs = configs;
            _resources = resources;
            _characterWindow = characterWindow;
            _classBookResource = ResourceHandler.GetBookResourceType(character.Config.Class);

            Name = character.Config.Name;
            Image = character.Config.Image;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            BooksAmount = resources.GetAmount(ResourceType.Book);
            ClassBooksAmount = resources.GetAmount(_classBookResource);
            OnOpenStats = OpenStats;
            OnUpgradeSkill = UpgradeSkill;

            AddDisposables(_selectedSkill, _skillName, _skillDescription, _skillUpgradePrice, _canUpgradeSkill);
            CreateSkills();
            AddDisposable(character.SkillLevel.Subscribe(_ => RefreshSkills()));
            AddDisposable(character.Stars.Subscribe(_ => RefreshSkills()));
            AddDisposable(ClassBooksAmount.Subscribe(_ => RefreshUpgradeState()));
            SelectFirstSkill();
            RefreshSkills();
        }

        private void CreateSkills()
        {
            foreach (var slot in SlotOrder)
            {
                var skillID = Model.Config.GetSkill(slot);
                var skill = GetSkill(skillID);
                var skillName = GetSkillName(skillID);
                var viewModel = CreateViewModel<SkillItemViewModel, SkillItemModel>(
                    new SkillItemModel(slot, skillID, skill, skillName));
                viewModel.OnSelect = () => SelectSkill(viewModel);
                _skills.Add(viewModel);
            }
        }

        private void SelectFirstSkill()
        {
            foreach (var skill in _skills)
            {
                if (!skill.IsVisible.Value) continue;
                SelectSkill(skill);
                return;
            }

            SelectSkill(null);
        }

        private void SelectSkill(SkillItemViewModel skill)
        {
            if (skill != null && !skill.IsVisible.Value) return;

            _selectedSkill.Value = skill;
            foreach (var item in _skills)
                item.SetSelected(item == skill);

            RefreshSelectedSkill();
        }

        private void RefreshSkills()
        {
            foreach (var skill in _skills)
                skill.SetLevel(GetSkillLevel(skill.Slot));

            RefreshSelectedSkill();
        }

        private void RefreshSelectedSkill()
        {
            var selected = _selectedSkill.Value;
            if (selected == null)
            {
                _skillName.Value = "";
                _skillDescription.Value = "";
                RefreshUpgradeState();
                return;
            }

            _skillName.Value = $"{selected.Name} Lv.{selected.Level.Value}";
            _skillDescription.Value = selected.Description;
            RefreshUpgradeState();
        }

        private int GetSkillLevel(SkillSlotType slot)
        {
            var maxLevel = Model.GetMaxSkillLevel(slot);
            return maxLevel <= 0 ? 0 : Mathf.Min(Model.SkillLevel.Value, maxLevel);
        }

        private void RefreshUpgradeState()
        {
            var price = _configs.Settings.SkillUpPrice;
            _skillUpgradePrice.Value = $"{ClassBooksAmount.Value}/{price}";

            var selected = _selectedSkill.Value;
            var maxLevel = selected == null ? 0 : Model.GetMaxSkillLevel(selected.Slot);
            _canUpgradeSkill.Value = selected != null
                                     && selected.IsVisible.Value
                                     && maxLevel > 0
                                     && Model.SkillLevel.Value < maxLevel
                                     && (price <= 0 || ClassBooksAmount.Value >= price);
        }

        private void UpgradeSkill()
        {
            var selected = _selectedSkill.Value;
            if (selected == null) return;

            var maxLevel = Model.GetMaxSkillLevel(selected.Slot);
            if (maxLevel <= 0 || Model.SkillLevel.Value >= maxLevel) return;

            var price = _configs.Settings.SkillUpPrice;
            if (price > 0 && ClassBooksAmount.Value < price) return;

            if (price > 0) _resources.Spend(_classBookResource, price);
            Model.UpgradeSkill();
        }

        private void OpenStats()
        {
            _characterWindow.ShowWindow(Model);
            Close();
        }

        private ISkillData GetSkill(uint skillID)
        {
            if (skillID == 0) return null;

            foreach (var skill in _configs.Skills.GetAll())
            {
                if (skill != null && skill.ID == skillID) return skill;
            }

            return null;
        }

        private string GetSkillName(uint skillID)
        {
            if (skillID == 0) return "";

            var skills = _configs.Skills.GetAll();
            var skillConfig = _configs.Skills as ConfigCore;

            for (var i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                if (skill == null || skill.ID != skillID) continue;

                if (skillConfig?.IDS != null && i < skillConfig.IDS.Count)
                    return skillConfig.IDS[i];

                return skillID.ToString();
            }

            return skillID.ToString();
        }

        public override void Dispose()
        {
            base.Dispose();
            OnOpenStats = null;
            OnUpgradeSkill = null;
        }
    }
}
