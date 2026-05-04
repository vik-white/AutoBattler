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
        public IReadOnlyReactiveProperty<float> Health;
        public List<ResourceViewModel> Resources = new ();
        public Sprite Image;
        public Sprite AbilityImage;
        public string AbilityDescription;
        public UnityAction OnUpgrade;
        public int Price = 30;
        
        public CharacterWindowViewModel(Character character, IConfigs configs, IResourceService resource) : base(character)
        {
            _resource = resource;
            Name = character.ID;
            Level = character.Level;
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
            OnUpgrade = Upgrade;
        }

        private void Upgrade()
        {
            if (_resource.GetAmount(ResourceType.Gold).Value < Price) return; 
            _resource.Spend(ResourceType.Gold, Price);
            Model.Upgrade();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnUpgrade = null;
        }
    }
}
