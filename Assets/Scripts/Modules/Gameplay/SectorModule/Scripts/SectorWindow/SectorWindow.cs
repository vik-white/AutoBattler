namespace vikwhite
{
    public interface ISectorWindow : IWindowPresenter
    {
        void ShowWindow(SectorPlayer sectorPlayer);
    }

    public class SectorWindow : WindowPresenter<SectorWindowView, SectorWindowViewModel>, ISectorWindow
    {
        public override string AssetName => "UI/Prefabs/SectorWindow/SectorWindow";

        public void ShowWindow(SectorPlayer sectorPlayer)
        {
            var window = _viewModelFactory.CreateViewModel<SectorWindowViewModel, SectorPlayer>(sectorPlayer);
            ShowWindow(window);
        }
    }
}
