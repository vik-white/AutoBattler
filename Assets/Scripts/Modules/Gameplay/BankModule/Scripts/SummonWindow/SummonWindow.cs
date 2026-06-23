namespace vikwhite
{
    public class SummonWindow : WindowPresenter<SummonWindowView, SummonWindowViewModel>, ISummonWindow
    {
        public override string AssetName => "UI/Prefabs/BankWindow/SummonWindow";

        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<SummonWindowViewModel>();
            ShowWindow(window);
        }
    }
}
