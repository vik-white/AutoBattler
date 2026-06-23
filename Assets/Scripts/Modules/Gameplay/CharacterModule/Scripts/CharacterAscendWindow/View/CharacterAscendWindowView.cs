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
            _view.ShardBarIcon.sprite = viewModel.ShardIcon;
            _view.RedeemShardIcon.sprite = viewModel.ShardIcon;
            _view.HeroShardIcon.sprite = viewModel.HeroShardIcon;

            _view.ShardBar.type = Image.Type.Filled;
            _view.ShardBar.fillMethod = Image.FillMethod.Horizontal;
            _view.ShardBar.fillOrigin = (int)Image.OriginHorizontal.Left;

            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.AscendButton, viewModel.OnAscend);
            BindClick(_view.PreviousStarButton, viewModel.OnSelectPreviousStar);
            BindClick(_view.NextStarButton, viewModel.OnSelectNextStar);

            Bind(viewModel.ShardPrice, value => _view.ShardPrice.text = value);
            Bind(viewModel.ShardProgress, value => _view.ShardBar.fillAmount = value);
            Bind(viewModel.CanSelectPreviousStar, value => _view.PreviousStarButton.interactable = value);
            Bind(viewModel.CanSelectNextStar, value => _view.NextStarButton.interactable = value);
            Bind(viewModel.RedeemResource.Amount, value => _view.RedeemAmount.text = value.ToString());

            CreateView<SelectableStarsView, StarsHierarchy>(_view.Stars).Initialize(viewModel.Stars);
            CreateView<StatsInfoView, StatsInfoHierarchy>(_view.StatsInfo).Initialize(viewModel.StatsInfo);
        }
    }
}
