using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class CardView : WindowView<CardHierarchy, CardViewModel>
    {
        private readonly IConfigs _configs;
        
        public CardView(GameObject view, IConfigs configs) : base(view)
        {
            _configs = configs;
        }

        protected override void UpdateViewModel(CardViewModel viewModel)
        {
            _view.ID = viewModel.ID;
            _view.Name.text = viewModel.ID;
            _view.Character.sprite = _configs.Characters.Get(viewModel.ID).Image;
        }
    }
}