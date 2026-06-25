using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadWindowViewModel: WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        public SquadItemViewModel[] Squad = new SquadItemViewModel[5];
        public List<SquadItemViewModel> Characters { get; } = new();
        public UnityAction<int, string> OnSetCharacter;
        public UnityAction<int> OnRemoveCharacter;
        public UnityAction OnFight;
        
        public SquadWindowViewModel(ISquadService squad, IEnvironmentStateMachine environmentStateMachine, ICharactersService characters)
        {
            _environmentStateMachine = environmentStateMachine;
            
            foreach (var character in characters.GetCharacters())
                Characters.Add(CreateViewModel<SquadItemViewModel, Character>(character));

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
