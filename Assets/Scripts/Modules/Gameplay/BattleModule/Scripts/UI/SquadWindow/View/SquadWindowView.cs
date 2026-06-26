using DG.Tweening;
using UnityEngine;

namespace vikwhite
{
    public class SquadWindowView : WindowView<SquadWindowHierarchy, SquadWindowViewModel>
    {
        private readonly ISquadItemViewFactory _squadItemViewFactory;
        
        public SquadWindowView(GameObject view, ISquadItemViewFactory squadItemViewFactory) : base(view)
        {
            _squadItemViewFactory = squadItemViewFactory;
        }
        
        protected override void UpdateViewModel(SquadWindowViewModel viewModel)
        {
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
                .Join(CreateAnchoredPositionYTween(_view.Top, _view.Top.anchoredPosition.y + 92f).SetEase(Ease.OutCubic))
                .Join(CreateAnchoredPositionYTween(_view.Bottom, _view.Bottom.anchoredPosition.y - 600f).SetEase(Ease.OutCubic));
        }
        
        private static Tween CreateAnchoredPositionYTween(RectTransform target, float endValue)
        {
            return DOTween.To(
                () => target.anchoredPosition.y,
                y =>
                {
                    Vector2 position = target.anchoredPosition;
                    position.y = y;
                    target.anchoredPosition = position;
                },
                endValue,
                1);
        }
    }
}