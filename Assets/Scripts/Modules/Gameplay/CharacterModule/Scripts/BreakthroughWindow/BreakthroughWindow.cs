namespace vikwhite
{
    public interface IBreakthroughWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }

    public class BreakthroughWindow :
        WindowPresenter<BreakthroughWindowView, BreakthroughWindowViewModel>,
        IBreakthroughWindow
    {
        public override string AssetName => "UI/Prefabs/BreakthroughWindow/BreakthroughWindow";

        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<BreakthroughWindowViewModel, Character>(character);
            ShowWindow(window);
        }
    }
}
