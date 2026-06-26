using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadWindowViewModel: WindowViewModel
    {
        private readonly ISquadService _squad;
        private readonly ISquadWindow _squadWindow;
        private readonly IStateMachine<IBattleState> _battleStateMachine;
        private readonly IEnvironmentStateMachine _environmentStateMachine;
        private readonly ReactiveProperty<bool> _canFight = new(false);

        public List<SquadItemViewModel> Characters { get; } = new();
        public IReadOnlyReactiveProperty<bool> CanFight => _canFight;
        public IReadOnlyReactiveProperty<int> PlayerMight => _squad.PlayerMight;
        public IReadOnlyReactiveProperty<int> EnemyMight => _squad.EnemyMight;
        public UnityAction OnFight;
        
        public SquadWindowViewModel(
            ISquadService squad,
            ISquadWindow squadWindow,
            ICharactersService characters,
            IStateMachine<IBattleState> battleStateMachine,
            IEnvironmentStateMachine environmentStateMachine)
        {
            _squad = squad;
            _squadWindow = squadWindow;
            _battleStateMachine = battleStateMachine;
            _environmentStateMachine = environmentStateMachine;
            
            foreach (var character in characters.GetCharacters())
            {
                var item = CreateViewModel<SquadItemViewModel, Character>(character);
                item.OnSelect = () => ToggleCharacter(item);
                item.SetSelected(_squad.IsSelected(character));
                Characters.Add(item);
            }

            AddDisposable(_squad.SelectedCount.Subscribe(count => _canFight.Value = count > 0));
            OnFight = StartFight;
        }

        public void StartFight()
        {
            if (_squad.SelectedCount.Value == 0) return;
            _battleStateMachine.SwitchState<IBattleStartState>();
        }

        public override void Close()
        {
            base.Close();
            _environmentStateMachine.SwitchState(EnvironmentType.Sector);
        }

        private void ToggleCharacter(SquadItemViewModel item)
        {
            if (_squad.IsSelected(item.Model))
            {
                _squad.Deselect(item.Model);
                item.SetSelected(false);
                return;
            }

            if (_squad.TrySelect(item.Model))
                item.SetSelected(true);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnFight = null;
            _canFight.Dispose();
        }
    }
}
