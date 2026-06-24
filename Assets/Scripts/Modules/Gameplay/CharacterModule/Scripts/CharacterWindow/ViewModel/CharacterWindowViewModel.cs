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
        private readonly ICharacterAscendWindow _characterAscendWindow;
        private readonly ICharacterSkillsWindow _characterSkillsWindow;
        private readonly ICharacterWindow _characterWindow;
        private readonly List<Character> _characters;
        public string Name;
        public IReadOnlyReactiveProperty<int> Level;
        public IReadOnlyReactiveProperty<int> Might;
        public ResourceViewModel ExpResources;
        public Sprite Image;
        public UnityAction OnUpgradeLevel;
        public UnityAction OnOpenUpgradeInfo;
        public UnityAction OnOpenAscendInfo;
        public UnityAction OnOpenSkills;
        public UnityAction OnSelectPreviousCharacter;
        public UnityAction OnSelectNextCharacter;
        public int LevelUpPrice;
        public RarityType Rarity;
        public Sprite ClassIcon;
        public StarsViewModel Stars { get; }
        
        public CharacterWindowViewModel(
            Character character,
            IConfigs configs,
            IResourceService resource,
            ICharacterUpgradeWindow characterUpgradeWindow,
            ICharacterAscendWindow characterAscendWindow,
            ICharactersService charactersService,
            ICharacterSkillsWindow characterSkillsWindow,
            ICharacterWindow characterWindow) : base(character)
        {
            _resource = resource;
            _configs = configs;
            _characterUpgradeWindow = characterUpgradeWindow;
            _characterAscendWindow = characterAscendWindow;
            _characterSkillsWindow = characterSkillsWindow;
            _characterWindow = characterWindow;
            _characters = new List<Character>(charactersService.GetCharacters());
            Name = character.Config.Name;
            Level = character.Level;
            Might = character.Might;
            LevelUpPrice = configs.Settings.LevelUpPrice;
            var config = configs.Characters.Get(character.ID);
            Image = config.Image;
            Rarity = config.Rarity;
            ClassIcon = configs.UI.ClassIcons[character.Config.Class];
            Stars = CreateViewModel<StarsViewModel, StarsModel>(new StarsModel(character.Stars));
            ExpResources = CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Exp));
            OnUpgradeLevel = LevelUpgrade;
            OnOpenUpgradeInfo = OpenUpgradeInfo;
            OnOpenAscendInfo = OpenAscendInfo;
            OnOpenSkills = OpenSkills;
            OnSelectPreviousCharacter = SelectPreviousCharacter;
            OnSelectNextCharacter = SelectNextCharacter;
        }

        private void OpenUpgradeInfo() => _characterUpgradeWindow.ShowWindow(Model);

        private void OpenAscendInfo() => _characterAscendWindow.ShowWindow(Model);

        private void OpenSkills()
        {
            _characterSkillsWindow.ShowWindow(Model);
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
            _characterWindow.ShowWindow(_characters[nextIndex]);
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
            OnOpenUpgradeInfo = null;
            OnOpenAscendInfo = null;
            OnOpenSkills = null;
            OnSelectPreviousCharacter = null;
            OnSelectNextCharacter = null;
        }
    }
}
