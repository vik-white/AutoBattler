namespace vikwhite
{
    public class CharacterModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ICharacterFactory, CharacterFactory>();
            Register<ICharactersService, CharactersService>();
            Register<Character>();
            
            Register<ICharacterWindow, CharacterWindow>();
            Register<CharacterWindowViewModel>();
            Register<CharacterWindowView>();
        }
    }
}