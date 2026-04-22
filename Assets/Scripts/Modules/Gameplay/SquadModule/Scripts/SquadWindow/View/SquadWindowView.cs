using System.Linq;
using UnityEngine;

namespace vikwhite
{
    public class SquadWindowView : WindowView<SquadWindowHierarchy, SquadWindowViewModel>
    {
        private readonly ICardViewFactory _cardViewFactory;
        
        public SquadWindowView(GameObject view, ICardViewFactory cardViewFactory) : base(view)
        {
            _cardViewFactory = cardViewFactory;
        }
        
        protected override void UpdateViewModel(SquadWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            ClearCardsContainer();
            InitializeCardsContainer();
            foreach (var container in _view.SquadContainers) InitializeSquadContainer(container);
            foreach (var card in viewModel.Cards) _cardViewFactory.Get(card, _view.CharacterContainer.Container);
            for (int i = 0; i < viewModel.Squad.Length; i++)
            {
                if(viewModel.Squad[i] != null) _cardViewFactory.Get(viewModel.Squad[i], _view.SquadContainers[i].Container);
            }
        }
        
        private void ClearCardsContainer() {
            foreach (Transform child in _view.CharacterContainer.Container) 
                GameObject.Destroy(child.gameObject);
        }
        
        private void InitializeCardsContainer() {
            _view.CharacterContainer.OnAddElement = (e) => {
                if (_view.SquadContainers.Contains(e.SourceContainer)) {
                    int index = _view.SquadContainers.IndexOf(e.SourceContainer);
                    ((SquadWindowViewModel)ViewModel).OnRemoveCharacter?.Invoke(index);
                }
                return true;
            };
        }

        private void InitializeSquadContainer(UIDropContainer container) {
            container.OnRemoveElement = (e) => {
                int index = _view.SquadContainers.IndexOf(container);
                ((SquadWindowViewModel)ViewModel).OnRemoveCharacter?.Invoke(index);
            };
            container.OnAddElement = (e) => {
                int index = _view.SquadContainers.IndexOf(container);
                var cardsView = container.Container.GetComponentsInChildren<CardHierarchy>();
                if (cardsView.Length > 1) {
                    var cardView = cardsView.FirstOrDefault(c => c != e);
                    cardView.transform.SetParent(_view.CharacterContainer.Container);
                    ((SquadWindowViewModel)ViewModel).OnRemoveCharacter?.Invoke(index);
                }
                ((SquadWindowViewModel)ViewModel).OnSetCharacter?.Invoke(index, e.ID);
                e.transform.localPosition = Vector3.zero;
                return true;
            };
        }
    }
}