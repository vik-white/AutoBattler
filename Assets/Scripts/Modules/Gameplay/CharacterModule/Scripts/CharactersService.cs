using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface ICharactersService
    {
        void Initialize();
        Character GetCharacter(string id);
        IReadOnlyCollection<Character> GetCharacters();
    }
    
    public class CharactersService : ICharactersService
    {
        private readonly IConfigs _configs;
        private readonly Dictionary<string, Character> _characters = new ();

        public CharactersService(IConfigs configs)
        {
            _configs = configs;
        }

        public void Initialize()
        {
            foreach (var characterData in _configs.Characters.GetAll())
            {
                if(characterData.Squad)
                    _characters.Add(characterData.ID, new Character(characterData.ID, 1));
            }
        }

        public Character GetCharacter(string id) => _characters[id];
        
        public IReadOnlyCollection<Character> GetCharacters() => _characters.Values;
    }
}