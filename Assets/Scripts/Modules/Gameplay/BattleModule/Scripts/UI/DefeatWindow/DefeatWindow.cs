using System.Collections.Generic;

namespace vikwhite
{
    public interface IDefeatWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class DefeatWindow : WindowPresenter<DefeatWindowView, DefeatWindowViewModel>, IDefeatWindow
    {
        public override string AssetName => "UI/Prefabs/DefeatWindow/DefeatWindow";
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<DefeatWindowViewModel>();
            ShowWindow(window);
        }
    }
}