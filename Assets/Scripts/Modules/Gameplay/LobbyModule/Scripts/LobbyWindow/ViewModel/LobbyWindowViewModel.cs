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
        public UnityAction OnMeta;
        
        public LobbyWindowViewModel(ISummonWindow summonWindow, IMetaWindow metaWindow, IEnvironmentStateMachine environmentStateMachine, IEventsService eventsService)
        {
            _environmentStateMachine = environmentStateMachine;
            OnSummon = summonWindow.ShowWindow;
            OnMeta = metaWindow.ShowWindow;
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
            OnMeta = null;
        }
    }
}
