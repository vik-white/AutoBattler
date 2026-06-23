using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterWindowViewModel: WindowViewModel<Character>
    {
        private readonly IResourceService _resource;
        private readonly IConfigs _configs;
        private readonly ICharacterUpgradeWindow _characterUpgradeWindow;
        public string Name;
        public IReadOnlyReactiveProperty<int> Level;
        public ResourceViewModel ExpResources;
        public Sprite Image;
        public UnityAction OnUpgradeLevel;
        public UnityAction OnOpenUpgradeInfo;
        public int LevelUpPrice;
        public RarityType Rarity;
        public Sprite ClassIcon;
        public StarsViewModel Stars { get; }
        
        public CharacterWindowViewModel(Character character, IConfigs configs, IResourceService resource, ICharacterUpgradeWindow characterUpgradeWindow) : base(character)
        {
            _resource = resource;
            _configs = configs;
            _characterUpgradeWindow = characterUpgradeWindow;
            Name = character.Config.Name;
            Level = character.Level;
            LevelUpPrice = configs.Settings.LevelUpPrice;
            var config = configs.Characters.Get(character.ID);
            Image = config.Image;
            Rarity = config.Rarity;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            Stars = CreateViewModel<StarsViewModel, IReadOnlyReactiveProperty<int>>(character.Stars);
            ExpResources = CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Exp));
            OnUpgradeLevel = LevelUpgrade;
            OnOpenUpgradeInfo = OpenUpgradeInfo;
        }

        private void OpenUpgradeInfo() => _characterUpgradeWindow.ShowWindow(Model);

        private void LevelUpgrade()
        {
            if (Model.GetMaxLevel() <= Model.Level.Value) return; 
            if (_resource.GetAmount(ResourceType.Exp).Value < LevelUpPrice) return; 
            _resource.Spend(ResourceType.Exp, LevelUpPrice);
            Model.UpgradeLevel();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnUpgradeLevel = null;
            OnOpenUpgradeInfo = null;
        }
    }
}
