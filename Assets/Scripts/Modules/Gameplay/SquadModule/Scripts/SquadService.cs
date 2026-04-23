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
        List<string> GetCharacters();
        FixedList32Bytes<uint> GetCharactersHash();
    }
    
    public class SquadServiceService : ISquadService
    {
        private readonly IProfileService _profile;
        private readonly IEventDispatcher _dispatcher;
        private List<string> _characters;

        public SquadServiceService(IProfileService profile, IEventDispatcher dispatcher)
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

        public FixedList32Bytes<uint> GetCharactersHash()
        {
            var ids = new FixedList32Bytes<uint>();
            foreach (var id in _characters)
            {
                ids.Add(id.CalculateHash32());
            }
            return ids;
        }
    }
}