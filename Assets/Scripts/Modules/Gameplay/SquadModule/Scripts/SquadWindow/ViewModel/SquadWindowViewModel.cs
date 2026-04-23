using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadWindowViewModel: WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        public CardViewModel[] Squad = new CardViewModel[5];
        public List<CardViewModel> Cards = new();
        public UnityAction<int, string> OnSetCharacter;
        public UnityAction<int> OnRemoveCharacter;
        public UnityAction OnFight;
        
        public SquadWindowViewModel(IConfigs configs, ISquadService squad, IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
            
            for (int i = 0; i < squad.GetCharacters().Count; i++)
            {
                var character = squad.GetCharacters()[i];
                if (character != "")
                {
                    var config = configs.Characters.Get(character);
                    Squad[i] = CreateViewModel<CardViewModel, ICharacterData>(config);
                }
            }
            
            foreach (var character in configs.Characters.GetAll())
            {
                if(!squad.GetCharacters().Contains(character.ID) && character.Squad )
                    Cards.Add(CreateViewModel<CardViewModel, ICharacterData>(character));
            }

            OnSetCharacter = squad.SetCharacter;
            OnRemoveCharacter = squad.SetCharacter;
            OnFight = StartFight;
        }

        public void StartFight()
        {
            _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSetCharacter = null;
            OnRemoveCharacter = null;
            OnFight = null;
        }
    }
}
