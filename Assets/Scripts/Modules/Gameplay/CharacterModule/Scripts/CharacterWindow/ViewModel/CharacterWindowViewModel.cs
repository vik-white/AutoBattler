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
        public string Name;
        public string Class;
        public string Rarity;
        public Color RarityColor;
        public IReadOnlyReactiveProperty<int> Level;
        public IReadOnlyReactiveProperty<int> Stars;
        public IReadOnlyReactiveProperty<int> SkillLevel;
        public IReadOnlyReactiveProperty<int> Shards;
        public IReadOnlyReactiveProperty<int> ClassShards;
        public IReadOnlyReactiveProperty<float> Health;
        public IReadOnlyReactiveProperty<float> Attack;
        public List<ResourceViewModel> Resources = new ();
        public Sprite Image;
        public Sprite AbilityImage;
        public string AbilityDescription;
        public UnityAction OnUpgradeLevel;
        public UnityAction OnSkillUpgrade;
        public UnityAction OnStarsUpgrade;
        public int LevelUpPrice;
        public int SkillUpPrice;
        
        public CharacterWindowViewModel(Character character, IConfigs configs, IResourceService resource, IClassShardService classShards) : base(character)
        {
            _resource = resource;
            Name = character.Config.Name;
            Level = character.Level;
            Stars = character.Stars;
            SkillLevel = character.SkillLevel;
            Shards = character.Shards;
            Health = character.Health;
            Attack = character.Attack;
            LevelUpPrice = configs.Settings.LevelUpPrice;
            SkillUpPrice = configs.Settings.SkillUpPrice;
            
            var config = configs.Characters.Get(character.ID);
            Image = config.Image;
            ClassShards = classShards.GetAmount(config.Class, config.Rarity);
            Class = config.Class.ToString();
            Rarity = config.Rarity.ToString();
            RarityColor = configs.RarityColors[config.Rarity];
            
            var activeSkillID = config.GetSkill(SkillSlotType.Active);
            foreach (var abilityData in configs.Skills.GetAll())
            {
                if (abilityData.ID != activeSkillID) continue;
                AbilityImage = abilityData.IconImage;
                AbilityDescription = abilityData.Description;
                break;
            }
            
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Gold)));
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Gem)));
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Book)));
            OnUpgradeLevel = LevelUpgrade;
            OnSkillUpgrade = SkillUpgrade;
            OnStarsUpgrade = StarsUpgrade;
        }

        private void LevelUpgrade()
        {
            if (Model.GetMaxLevel() <= Model.Level.Value) return; 
            if (_resource.GetAmount(ResourceType.Gold).Value < LevelUpPrice) return; 
            _resource.Spend(ResourceType.Gold, LevelUpPrice);
            Model.UpgradeLevel();
        }

        private void SkillUpgrade()
        {
            if (Model.GetMaxSkillLevel(SkillSlotType.Active) <= Model.SkillLevel.Value) return; 
            if (_resource.GetAmount(ResourceType.Book).Value < SkillUpPrice) return;
            _resource.Spend(ResourceType.Book, SkillUpPrice);
            Model.UpgradeSkill();
        }
        
        private void StarsUpgrade()
        {
            if(Model.Shards.Value < 5) return;
            Model.RemoveShards(5);
            Model.UpgradeStars();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnUpgradeLevel = null;
            OnSkillUpgrade = null;
            OnStarsUpgrade = null;
        }
    }
}
