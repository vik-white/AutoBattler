using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CharacterWindowViewModel: WindowViewModel<Character>
    {
        public string Name;
        public string Level;
        
        public CharacterWindowViewModel(Character character) : base(character)
        {
            Name = character.ID;
            Level = character.Level.ToString();
        }
        
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
