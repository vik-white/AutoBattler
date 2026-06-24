using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public enum MetaItemTipType
    {
        None,
        Upgrade,
        Stars,
        Skills,
    }

    public class MetaItemViewModel : WindowViewModel<Character>
    {
        private readonly IConfigs _configs;
        private readonly IResourceService _resources;
        private readonly ResourceType _classBookResource;
        private readonly ReactiveProperty<MetaItemTipType> _tip = new();

        public UnityAction OnSelect;
        public IReadOnlyReactiveProperty<int> Level { get; }
        public IReadOnlyReactiveProperty<MetaItemTipType> Tip => _tip;
        public StarsViewModel Stars { get; }
        public Sprite ClassIcon { get; }
        public Sprite RarityBackground { get; }
        public GameObject ImagePrefab { get; }

        public MetaItemViewModel(
            Character character,
            ICharacterWindow characterWindow,
            IConfigs configs,
            IResourceService resources) : base(character)
        {
            _configs = configs;
            _resources = resources;
            _classBookResource = ResourceHandler.GetBookResourceType(character.Config.Class);

            OnSelect = () => characterWindow.ShowWindow(character);
            Level = character.Level;
            Stars = CreateViewModel<StarsViewModel, StarsModel>(new StarsModel(character.Stars));
            ImagePrefab = character.Config.HeadPrefab;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            configs.UI.Rarities.TryGetValue(character.Config.Rarity, out var rarity);
            RarityBackground = rarity.MetaBG;

            AddDisposable(_tip);
            AddDisposable(character.Level.Subscribe(_ => RefreshTip()));
            AddDisposable(character.Stars.Subscribe(_ => RefreshTip()));
            AddDisposable(character.Shards.Subscribe(_ => RefreshTip()));
            foreach (var skill in character.Skills)
                AddDisposable(skill.Level.Subscribe(_ => RefreshTip()));
            AddDisposable(resources.GetAmount(ResourceType.Exp).Subscribe(_ => RefreshTip()));
            AddDisposable(resources.GetAmount(_classBookResource).Subscribe(_ => RefreshTip()));
        }

        private void RefreshTip()
        {
            _tip.Value = CanUpgradeStars()
                ? MetaItemTipType.Stars
                : CanUpgradeSkill()
                    ? MetaItemTipType.Skills
                    : CanUpgradeLevel()
                        ? MetaItemTipType.Upgrade
                        : MetaItemTipType.None;
        }

        private bool CanUpgradeStars()
        {
            var price = _configs.Settings.StarUpPrice;
            return Model.Stars.Value < _configs.Stars.GetAll().Count
                   && (price <= 0 || Model.Shards.Value >= price);
        }

        private bool CanUpgradeSkill()
        {
            var price = _configs.Settings.SkillUpPrice;
            if (price > 0 && _resources.GetAmount(_classBookResource).Value < price) return false;

            foreach (var slot in SkillSlotExtensions.UpgradableSlots)
            {
                var skill = Model.GetSkill(slot);
                if (skill == null || skill.Config == null) continue;
                var maxLevel = Model.GetMaxSkillLevel(slot);
                if (maxLevel > 0 && skill.Level.Value < maxLevel) return true;
            }

            return false;
        }

        private bool CanUpgradeLevel()
        {
            var price = _configs.Settings.LevelUpPrice;
            return Model.Level.Value < Model.GetMaxLevel()
                   && (price <= 0 || _resources.GetAmount(ResourceType.Exp).Value >= price);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}
