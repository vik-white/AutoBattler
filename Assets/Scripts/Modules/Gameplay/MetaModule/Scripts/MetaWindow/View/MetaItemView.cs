using UnityEngine;
using Object = UnityEngine.Object;

namespace vikwhite
{
    public class MetaItemView : WindowView<MetaItemHierarchy, MetaItemViewModel>
    {
        public MetaItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(MetaItemViewModel viewModel)
        {
            BindClick(_view.Button, viewModel.OnSelect);
            Bind(viewModel.Level, level => _view.Level.text = $"Lv. {level}");
            Bind(viewModel.Tip, tip =>
            {
                _view.TipStars.SetActive(tip == MetaItemTipType.Stars);
                _view.TipSkills.SetActive(tip == MetaItemTipType.Skills);
                _view.TipUpgrade.SetActive(tip == MetaItemTipType.Upgrade);
            });
            CreateView<StarsView, StarsHierarchy>(_view.Stars).Initialize(viewModel.Stars);

            if (viewModel.RarityBackground != null)
                _view.Background.sprite = viewModel.RarityBackground;
            
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            _view.ClassIcon.enabled = viewModel.ClassIcon != null;
            
            SetCharacterImage(viewModel.ImagePrefab);
        }

        private void SetCharacterImage(GameObject imagePrefab)
        {
            _view.ImageContainer.ClearChildren();
            if (imagePrefab == null) return;
            
            Object.Instantiate(imagePrefab, _view.ImageContainer, false);
        }

    }
}
