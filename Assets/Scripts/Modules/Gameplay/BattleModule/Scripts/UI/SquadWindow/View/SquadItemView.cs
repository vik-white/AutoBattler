using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadItemView : WindowView<SquadItemHierarchy, SquadItemViewModel>
    {
        
        public SquadItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SquadItemViewModel viewModel)
        {
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            _view.RarityBG.sprite = viewModel.RarityBG;
            BindClick(_view.Button, viewModel.OnSelect);
            Bind(viewModel.Level, level => _view.Level.text = $"<size=17>Lv.</size> {level}");
            Bind(viewModel.IsSelected, OnSelected);
            _view.HeroContainer.ClearChildren();
            Object.Instantiate(viewModel.ImagePrefab, _view.HeroContainer, false);
        }

        private void OnSelected(bool selected)
        {
            _view.Selected.SetActive(selected);
            _view.Item.Color = selected ? new Color(0.5f,0.5f,0.5f) : new Color(1,1,1);
        }
    }
}
