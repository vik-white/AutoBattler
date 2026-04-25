namespace vikwhite
{
    public interface ICharacterWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class CharacterWindow : WindowPresenter<CharacterWindowView, CharacterWindowViewModel>, ICharacterWindow
    {
        public override string AssetName => "UI/Prefabs/CharacterWindow/CharacterWindow";
        
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<CharacterWindowViewModel>();
            ShowWindow(window);
        }
    }
}