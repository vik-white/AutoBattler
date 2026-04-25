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
        
        public SquadWindowViewModel(ISquadService squad, IEnvironmentStateMachine environmentStateMachine, ICharactersService characters)
        {
            _environmentStateMachine = environmentStateMachine;
            
            for (int i = 0; i < squad.GetCharacters().Count; i++)
            {
                var character = squad.GetCharacters()[i];
                if (character != null) Squad[i] = CreateViewModel<CardViewModel, Character>(character);
            }
            
            foreach (var character in characters.GetCharacters())
            {
                if(!squad.GetCharacters().Contains(character))
                    Cards.Add(CreateViewModel<CardViewModel, Character>(character));
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
