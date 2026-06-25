using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadItemView : WindowView<SquadItemHierarchy, SquadItemViewModel>
    {
        
        public SquadItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SquadItemViewModel viewModel)
        {
            _view.ID = viewModel.ID;
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            _view.RarityBG.sprite = viewModel.RarityBG;
            BindClick(_view.Button, viewModel.OnSelect);
            Bind(viewModel.Level, level => _view.Level.text = $"{level} Lv");
            _view.HeroContainer.ClearChildren();
            Object.Instantiate(viewModel.ImagePrefab, _view.HeroContainer, false);
        }
    }
}