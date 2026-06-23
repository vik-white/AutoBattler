using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace vikwhite
{
    public class CharacterAscendWindowView : WindowView<CharacterAscendHierarchy, CharacterAscendWindowViewModel>
    {
        public CharacterAscendWindowView(GameObject view) : base(view)
        {
        }

        protected override void UpdateViewModel(CharacterAscendWindowViewModel viewModel)
        {
            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            _view.ShardBarIcon.sprite = viewModel.ShardBarIcon;
            _view.RedeemShardIcon.sprite = viewModel.RedeemShardIcon;
            _view.HeroShardIcon.sprite = viewModel.HeroShardIcon;

            SetupShardBar();

            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.AscendButton, viewModel.OnAscend);
            BindClick(_view.PreviousStarButton, viewModel.OnSelectPreviousStar);
            BindClick(_view.NextStarButton, viewModel.OnSelectNextStar);
            BindClick(_view.RedeemButton, viewModel.OnRedeemShard);

            Bind(viewModel.ShardPrice, value => _view.ShardPrice.text = value);
            Bind(viewModel.ShardProgress, value => _view.ShardBar.fillAmount = value);
            Bind(viewModel.CanSelectPreviousStar, value => SetInteractable(_view.PreviousStarButton, value));
            Bind(viewModel.CanSelectNextStar, value => SetInteractable(_view.NextStarButton, value));
            Bind(viewModel.RedeemResource.Amount, value => _view.RedeemAmount.text = value.ToString());

            CreateView<SelectableStarsView, StarsHierarchy>(_view.Stars).Initialize(viewModel.Stars);
            CreateView<StatsInfoView, StatsInfoHierarchy>(_view.StatsInfo).Initialize(viewModel.StatsInfo);
        }

        private void SetupShardBar()
        {
            _view.ShardBar.type = Image.Type.Filled;
            _view.ShardBar.fillMethod = Image.FillMethod.Horizontal;
            _view.ShardBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }

        private void BindClick(RectTransform buttonRoot, UnityAction onClick)
        {
            if (buttonRoot == null)
            {
                return;
            }

            var button = buttonRoot.GetComponent<Button>();

            if (button == null)
            {
                return;
            }

            BindClick(button, onClick);
        }

        private void SetInteractable(RectTransform buttonRoot, bool interactable)
        {
            if (buttonRoot == null)
            {
                return;
            }

            var button = buttonRoot.GetComponent<Button>();

            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
        }
    }
}
