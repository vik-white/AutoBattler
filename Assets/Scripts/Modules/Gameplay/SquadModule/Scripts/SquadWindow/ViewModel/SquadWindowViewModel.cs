using System.Collections.Generic;
using UniRx;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadWindowViewModel: WindowViewModel
    {
        private readonly ISquadService _squad;
        private readonly ReactiveProperty<bool> _canFight = new(false);

        public List<SquadItemViewModel> Characters { get; } = new();
        public IReadOnlyReactiveProperty<bool> CanFight => _canFight;
        public UnityAction OnFight;
        
        public SquadWindowViewModel(ISquadService squad, ICharactersService characters)
        {
            _squad = squad;
            
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
            _squad.RequestFight();
            base.Close();
        }

        public override void Close()
        {
            base.Close();
            _squad.RequestBack();
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
