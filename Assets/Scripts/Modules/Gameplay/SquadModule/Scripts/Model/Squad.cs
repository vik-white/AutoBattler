using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    public interface ISquad
    {
        void Initialize();
        void SetCharacter(int index);
        void SetCharacter(int index, string id);
        List<string> GetCharacters();
    }
    
    public class Squad : ISquad
    {
        private readonly IProfileService _profile;
        private readonly IEventDispatcher _dispatcher;
        private List<string> _characters;

        public Squad(IProfileService profile, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _characters = new (_profile.Data.Squad);
        }

        public void SetCharacter(int index) => SetCharacter(index, "");

        public void SetCharacter(int index, string id)
        {
            _characters[index] = id;
            _dispatcher.Dispatch(new SetSquadCharacterEvent(index, id));
        }
        
        public List<string> GetCharacters() => _characters;
    }
}