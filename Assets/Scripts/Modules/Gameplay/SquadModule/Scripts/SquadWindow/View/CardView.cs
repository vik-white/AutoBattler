using UnityEngine;

namespace vikwhite
{
    public class CardView : WindowView<CardHierarchy, CardViewModel>
    {
        public CardView(GameObject view) : base(view)
        {
        }

        protected override void UpdateViewModel(CardViewModel viewModel)
        {
            _view.ID = viewModel.ID;
            _view.Name.text = viewModel.ID;
        }
    }
}