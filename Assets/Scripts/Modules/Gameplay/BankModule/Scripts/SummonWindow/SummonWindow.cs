namespace vikwhite
{
    public interface ISummonWindow : IWindowPresenter
    {
        void ShowWindow();
    }

    public class SummonWindow : WindowPresenter<SummonWindowView, SummonWindowViewModel>, ISummonWindow
    {
        public override string AssetName => "UI/Prefabs/SummonWindow/SummonWindow";

        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<SummonWindowViewModel>();
            ShowWindow(window);
        }
    }
}
