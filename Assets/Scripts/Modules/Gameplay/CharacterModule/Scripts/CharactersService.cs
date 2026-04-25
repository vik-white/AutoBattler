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
        private readonly IProfileService _profile;
        private readonly ICharacterFactory _factory;
        private readonly Dictionary<string, Character> _characters = new ();

        public CharactersService(IProfileService profile, ICharacterFactory factory)
        {
            _profile = profile;
            _factory = factory;
        }

        public void Initialize()
        {
            foreach (var characterData in _profile.Data.Characters)
                _characters.Add(characterData.ID, _factory.Create(characterData.ID, characterData.Level));
        }

        public Character GetCharacter(string id) => _characters[id];
        
        public IReadOnlyCollection<Character> GetCharacters() => _characters.Values;
    }
}