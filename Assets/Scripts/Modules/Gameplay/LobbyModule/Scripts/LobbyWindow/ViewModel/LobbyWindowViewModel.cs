using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class LobbyWindowViewModel: WindowViewModel
    {
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        public List<EventItemViewModel> Events = new ();
        private readonly ReadOnlyReactiveProperty<int> _might;
        public IReadOnlyReactiveProperty<int> Might => _might;
        public IReadOnlyReactiveProperty<int> Gems { get; }
        public ResourceViewModel Gold { get; }
        public UnityAction OnAdventure;
        public UnityAction OnSummon;
        public UnityAction OnMeta;
        
        public LobbyWindowViewModel(
            ISummonWindow summonWindow,
            IMetaWindow metaWindow,
            IEnvironmentStateMachine environmentStateMachine,
            IEventsService eventsService,
            ICharactersService charactersService,
            IResourceService resource)
        {
            _environmentStateMachine = environmentStateMachine;
            OnSummon = summonWindow.ShowWindow;
            OnMeta = metaWindow.ShowWindow;
            OnAdventure = OpenAdventure;

            var characterMight = charactersService.GetCharacters().Select(character => character.Might).ToList();
            _might = characterMight.CombineLatest().Select(values => values.Sum()).ToReadOnlyReactiveProperty();
            AddDisposable(_might);
            
            Gems = resource.GetAmount(ResourceType.Gem);
            Gold = CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.Gold));

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
