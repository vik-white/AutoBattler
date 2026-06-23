namespace vikwhite
{
    public interface ICharacterUpgradeWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }

    public class CharacterUpgradeWindow : WindowPresenter<CharacterUpgradeWindowView, CharacterUpgradeWindowViewModel>, ICharacterUpgradeWindow
    {
        public override string AssetName => "UI/Prefabs/CharacterWindow/CharacterUpgradeWindow";

        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<CharacterUpgradeWindowViewModel, Character>(character);
            ShowWindow(window);
        }
    }
}
