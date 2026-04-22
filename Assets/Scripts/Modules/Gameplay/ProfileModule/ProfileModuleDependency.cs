namespace vikwhite
{
    public class ProfileModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IProfileService, ProfileService>();
            Register<IEventHandler, SetSquadCharacterProfileHandler>();
        }
    }
}