namespace vikwhite
{
    public interface ICharacterSkillsWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }

    public class CharacterSkillsWindow : WindowPresenter<CharacterSkillsWindowView, CharacterSkillsWindowViewModel>, ICharacterSkillsWindow
    {
        public override string AssetName => "UI/Prefabs/CharacterWindow/CharacterSkillsWindow";

        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<CharacterSkillsWindowViewModel, Character>(character);
            ShowWindow(window);
        }
    }
}
