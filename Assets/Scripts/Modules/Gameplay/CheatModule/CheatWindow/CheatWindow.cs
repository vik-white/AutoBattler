using System.Collections.Generic;

namespace vikwhite
{
    public interface ICheatWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class CheatWindow : WindowPresenter<CheatWindowView, CheatWindowViewModel>, ICheatWindow
    {
        public override string AssetName => "UI/Prefabs/CheatWindow/CheatWindow";
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<CheatWindowViewModel>();
            ShowWindow(window);
        }
    }
}