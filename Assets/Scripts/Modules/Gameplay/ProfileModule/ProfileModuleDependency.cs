namespace vikwhite
{
    public class ProfileModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IProfileService, ProfileService>();
            Register<IEventHandler, SetSquadCharacterProfileHandler>();
            Register<IEventHandler, SetRoadMapLocationProfileHandler>();
            Register<IEventHandler, ChangeResourceProfileHandler>();
            Register<IEventHandler, ChangeCharacterLevelProfileHandler>();
            Register<IEventHandler, ChangeCharacterShardProfileHandler>();
        }
    }
}