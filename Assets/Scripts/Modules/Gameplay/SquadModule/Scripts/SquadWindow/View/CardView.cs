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
            _view.Name.text = viewModel.Name;
            _view.Character.sprite = _configs.Characters.Get(viewModel.ID).Image;
            _view.Rarity.color = viewModel.RarityColor;
            BindClick(_view.Button, viewModel.OnSelect);
            Bind(viewModel.Level, level => _view.Level.text = $"{level} Lv");
        }
    }
}