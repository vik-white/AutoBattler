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
        public IReadOnlyReactiveProperty<int> Level;
        public IReadOnlyReactiveProperty<int> Stars;
        public IReadOnlyReactiveProperty<int> SkillLevel;
        public IReadOnlyReactiveProperty<int> Shards;
        public IReadOnlyReactiveProperty<float> Health;
        public List<ResourceViewModel> Resources = new ();
        public Sprite Image;
        public Sprite AbilityImage;
        public string AbilityDescription;
        public UnityAction OnUpgradeLevel;
        public UnityAction OnSkillUpgrade;
        public UnityAction OnStarsUpgrade;
        public int Price = 30;
        public int SkillPrice = 30;
        
        public CharacterWindowViewModel(Character character, IConfigs configs, IResourceService resource) : base(character)
        {
            _resource = resource;
            Name = character.ID;
            Level = character.Level;
            Stars = character.Stars;
            SkillLevel = character.SkillLevel;
            Shards = character.Shards;
            Health = character.Health;

            var config = configs.Characters.Get(character.ID);
            Image = config.Image;
            
            foreach (var abilityData in configs.Abilities.GetAll())
            {
                if (abilityData.AbilityID == config.ActiveAbility)
                {
                    AbilityImage = abilityData.IconImage;
                    AbilityDescription = abilityData.Description;
                    break;
                }
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
            if (_resource.GetAmount(ResourceType.Gold).Value < Price) return; 
            _resource.Spend(ResourceType.Gold, Price);
            Model.UpgradeLevel();
        }

        private void SkillUpgrade()
        {
            if (_resource.GetAmount(ResourceType.Book).Value < SkillPrice) return;
            _resource.Spend(ResourceType.Book, SkillPrice);
            Model.UpgradeSkill();
        }
        
        private void StarsUpgrade()
        {
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
