using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadWindowViewModel: WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        public List<SquadItemViewModel> Characters { get; } = new();
        public UnityAction OnFight;
        
        public SquadWindowViewModel(IEnvironmentStateMachine environmentStateMachine, ICharactersService characters)
        {
            _environmentStateMachine = environmentStateMachine;
            
            foreach (var character in characters.GetCharacters())
                Characters.Add(CreateViewModel<SquadItemViewModel, Character>(character));

            OnFight = StartFight;
        }

        public void StartFight()
        {
            _environmentStateMachine.SwitchState(EnvironmentType.Battle);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnFight = null;
        }
    }
}
