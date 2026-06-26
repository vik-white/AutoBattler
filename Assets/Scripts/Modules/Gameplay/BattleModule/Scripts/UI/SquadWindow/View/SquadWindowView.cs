using DG.Tweening;
using UnityEngine;

namespace vikwhite
{
    public class SquadWindowView : WindowView<SquadWindowHierarchy, SquadWindowViewModel>
    {
        private readonly ISquadItemViewFactory _squadItemViewFactory;
            private readonly Vector2 _topDefaultPosition;
            private readonly Vector2 _bottomDefaultPosition;
        
        public SquadWindowView(GameObject view, ISquadItemViewFactory squadItemViewFactory) : base(view)
        {
            _squadItemViewFactory = squadItemViewFactory;
            _topDefaultPosition = _view.Top.anchoredPosition;
            _bottomDefaultPosition = _view.Bottom.anchoredPosition;
        }
        
        protected override void UpdateViewModel(SquadWindowViewModel viewModel)
        {
            _view.Top.anchoredPosition = _topDefaultPosition;
            _view.Bottom.anchoredPosition = _bottomDefaultPosition;
            Bind(viewModel.PlayerMight, might => _view.PlayerMight.text = might.ToString());
            Bind(viewModel.EnemyMight, might => _view.EnemyMight.text = might.ToString());
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.FightButton, () =>
            {
                PlayBattleStartAnimation();
                viewModel.StartFight();
            });
            Bind(viewModel.CanFight, canFight => _view.FightButton.interactable = canFight);
            _view.SquadItemsContainer.ClearChildren();
            foreach (var character in viewModel.Characters)
                _squadItemViewFactory.Get(character, _view.SquadItemsContainer);
        }

        private void PlayBattleStartAnimation()
        {
            DOTween.Sequence()
                .SetUpdate(true)
                .Join(TweenHendler.CreateAnchoredPositionYTween(_view.Top, _topDefaultPosition.y + 92f).SetEase(Ease.OutCubic))
                .Join(TweenHendler.CreateAnchoredPositionYTween(_view.Bottom, _bottomDefaultPosition.y - 600f).SetEase(Ease.OutCubic));
        }
    }
}
