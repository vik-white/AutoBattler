using System;
using UnityEngine;

namespace vikwhite
{
    public class SectorWindowView : WindowView<SectorWindowHierarchy, SectorWindowViewModel>
    {
        public SectorWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SectorWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.OnLobby);
            BindClick(_view.FightButton, viewModel.OnFight);
            if (_view.GoToNextButton != null)
                BindClick(_view.GoToNextButton, viewModel.OnGoToNext);

            viewModel.Changed += Refresh;
            Register(new ActionDisposable(() => viewModel.Changed -= Refresh));
            Refresh();

            void Refresh()
            {
                if (_view.Location != null)
                    _view.Location.text = viewModel.CurrentLocation;
                if (_view.GoToNextButton != null)
                    _view.GoToNextButton.interactable = viewModel.CanGoToNext;
                if (_view.FightButton != null)
                    _view.FightButton.interactable = viewModel.CanFight;
            }
        }

        private sealed class ActionDisposable : IDisposable
        {
            private readonly Action _dispose;

            public ActionDisposable(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose?.Invoke();
            }
        }
    }
}
