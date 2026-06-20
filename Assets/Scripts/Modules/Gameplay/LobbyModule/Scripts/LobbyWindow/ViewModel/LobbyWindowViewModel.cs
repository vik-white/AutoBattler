using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        public List<EventItemViewModel> Events = new ();
        public UnityAction OnAdventure;
        public UnityAction OnSummon;
        
        public LobbyWindowViewModel(ISummonWindow summonWindow, IEnvironmentStateMachine environmentStateMachine, IEventsService eventsService)
        {
            _environmentStateMachine = environmentStateMachine;
            OnSummon = summonWindow.ShowWindow;
            OnAdventure = OpenAdventure;

            foreach (var gameEvent in eventsService.GetAll())
                Events.Add(CreateViewModel<EventItemViewModel, GameEvent>(gameEvent));
        }
        
        public void OpenAdventure()
        {
            _environmentStateMachine.SwitchState(EnvironmentType.Sector);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnAdventure = null;
            OnSummon = null;
        }
    }
}
