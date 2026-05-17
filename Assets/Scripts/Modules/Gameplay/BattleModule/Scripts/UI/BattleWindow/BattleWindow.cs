namespace vikwhite
{
    public interface IBattleWindow
    {
        void Show();
        void Hide();
    }

    public class BattleWindow : WindowPresenter<BattleWindowView, BattleWindowViewModel>, IBattleWindow
    {
        public override string AssetName => "UI/Prefabs/BattleWindow/BattleWindow";

        public void Show()
        {
            var viewModel = _viewModelFactory.CreateViewModel<BattleWindowViewModel>();
            ShowWindow(viewModel);
        }

        public void Hide()
        {
            CloseWindow();
        }
    }
}
