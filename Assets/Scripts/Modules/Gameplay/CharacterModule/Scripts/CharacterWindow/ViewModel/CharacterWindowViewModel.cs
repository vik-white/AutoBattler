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
        public string Name;
        public IReadOnlyReactiveProperty<int> Level;
        public ResourceViewModel ExpResources;
        public Sprite Image;
        public UnityAction OnUpgradeLevel;
        public int LevelUpPrice;
        public StarsViewModel Stars { get; }
        
        public CharacterWindowViewModel(Character character, IConfigs configs, IResourceService resource) : base(character)
        {
            _resource = resource;
            _configs = configs;
            Name = character.Config.Name;
            Level = character.Level;
            LevelUpPrice = configs.Settings.LevelUpPrice;
            var config = configs.Characters.Get(character.ID);
            Image = config.Image;
            Stars = CreateViewModel<StarsViewModel, IReadOnlyReactiveProperty<int>>(character.Stars);
            ExpResources = CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Exp));
            OnUpgradeLevel = LevelUpgrade;
        }

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
        }
    }
}
