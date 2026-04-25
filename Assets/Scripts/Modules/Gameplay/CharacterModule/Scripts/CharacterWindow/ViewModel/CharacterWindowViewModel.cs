using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterWindowViewModel: WindowViewModel<Character>
    {
        public string Name;
        public IReadOnlyReactiveProperty<int> Level;
        public IReadOnlyReactiveProperty<float> Health;
        public Sprite Image;
        public UnityAction OnUpgrade;
        
        public CharacterWindowViewModel(Character character, IConfigs configs) : base(character)
        {
            Name = character.ID;
            Level = character.Level;
            Health = character.Health;
            Image = configs.Characters.Get(character.ID).Image;
            OnUpgrade = character.Upgrade;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnUpgrade = null;
        }
    }
}
