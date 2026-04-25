namespace vikwhite
{
    public class CharacterModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<ICharactersService, CharactersService>();
            
            Register<ICharacterWindow, CharacterWindow>();
            Register<CharacterWindowViewModel>();
            Register<CharacterWindowView>();
        }
    }
}