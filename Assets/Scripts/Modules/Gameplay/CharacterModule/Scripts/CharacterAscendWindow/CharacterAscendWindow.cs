namespace vikwhite
{
    public interface ICharacterAscendWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }

    public class CharacterAscendWindow : WindowPresenter<CharacterAscendWindowView, CharacterAscendWindowViewModel>, ICharacterAscendWindow
    {
        public override string AssetName => "UI/Prefabs/CharacterWindow/CharacterAscendWindow";

        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<CharacterAscendWindowViewModel, Character>(character);

            ShowWindow(window);
        }
    }
}
