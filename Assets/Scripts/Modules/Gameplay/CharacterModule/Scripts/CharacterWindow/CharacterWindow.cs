namespace vikwhite
{
    public interface ICharacterWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }
    
    public class CharacterWindow : WindowPresenter<CharacterWindowView, CharacterWindowViewModel>, ICharacterWindow
    {
        public override string AssetName => "UI/Prefabs/CharacterWindow/CharacterWindow";
        
        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<CharacterWindowViewModel, Character>(character);
            ShowWindow(window);
        }
    }
}