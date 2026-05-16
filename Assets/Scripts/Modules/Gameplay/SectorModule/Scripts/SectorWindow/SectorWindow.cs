namespace vikwhite
{
    public interface ISectorWindow : IWindowPresenter
    {
        void ShowWindow();
    }

    public class SectorWindow : WindowPresenter<SectorWindowView, SectorWindowViewModel>, ISectorWindow
    {
        public override string AssetName => "UI/Prefabs/SectorWindow/SectorWindow";

        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<SectorWindowViewModel>();
            ShowWindow(window);
        }
    }
}
