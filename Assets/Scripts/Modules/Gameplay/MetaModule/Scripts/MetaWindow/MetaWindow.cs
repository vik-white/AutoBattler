namespace vikwhite
{
    public interface IMetaWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class MetaWindow : WindowPresenter<MetaWindowView, MetaWindowViewModel>, IMetaWindow
    {
        public override string AssetName => "UI/Prefabs/MetaWindow/MetaWindow";
        
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<MetaWindowViewModel>();
            ShowWindow(window);
        }
    }
}
