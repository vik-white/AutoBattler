using System.Collections.Generic;
using Rukhanka.Toolbox;
using Unity.Collections;
using UnityEngine;

namespace vikwhite
{
    public interface ISquadService
    {
        void Initialize();
        void SetCharacter(int index);
        void SetCharacter(int index, string id);
        List<Character> GetCharacters();
    }
    
    public class SquadServiceService : ISquadService
    {
        private readonly IProfileService _profile;
        private readonly IEventDispatcher _dispatcher;
        private readonly ICharactersService _charactersService;
        private List<Character> _characters;

        public SquadServiceService(IProfileService profile, IEventDispatcher dispatcher, ICharactersService charactersService)
        {
            _profile = profile;
            _dispatcher = dispatcher;
            _charactersService = charactersService;
        }

        public void Initialize()
        {
            _characters = new ();
            foreach (var characterID in _profile.Data.Squad)
            {
                if (characterID == null || characterID == "")
                    _characters.Add(null);
                else
                    _characters.Add(_charactersService.GetCharacter(characterID));
            }
        }

        public void SetCharacter(int index) => SetCharacter(index, "");

        public void SetCharacter(int index, string id)
        {
            _characters[index] = id != "" ? _charactersService.GetCharacter(id) : null;
            _dispatcher.Dispatch(new SetSquadCharacterEvent(index, id));
        }
        
        public List<Character> GetCharacters() => _characters;
    }
}