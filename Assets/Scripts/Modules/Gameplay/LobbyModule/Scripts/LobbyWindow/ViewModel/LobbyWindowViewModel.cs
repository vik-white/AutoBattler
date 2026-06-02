using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        public List<ResourceViewModel> Resources = new ();
        public List<EventItemViewModel> Events = new ();
        public UnityAction OnCheats;
        public UnityAction OnMap;
        public UnityAction OnBank;
        
        public LobbyWindowViewModel(ICheatWindow cheatWindow, ISummonWindow summonWindow, IResourceService resource, IEnvironmentStateMachine environmentStateMachine, IEventsService eventsService)
        {
            _environmentStateMachine = environmentStateMachine;
            OnCheats = cheatWindow.ShowWindow;
            OnBank = summonWindow.ShowWindow;
            OnMap = OpenMap;
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Gold)));
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Gem)));

            foreach (var gameEvent in eventsService.GetAll())
                Events.Add(CreateViewModel<EventItemViewModel, GameEvent>(gameEvent));
        }
        
        public void OpenMap()
        {
            _environmentStateMachine.SwitchState(EnvironmentType.Sector);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnCheats = null;
            OnMap = null;
            OnBank = null;
        }
    }
}
