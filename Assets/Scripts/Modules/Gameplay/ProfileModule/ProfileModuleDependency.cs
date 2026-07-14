namespace vikwhite
{
    public class ProfileModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IProfileService, ProfileService>();
            Register<IEventHandler, SetSectorLocationProfileHandler>();
            Register<IEventHandler, ChangeResourceProfileHandler>();
            Register<IEventHandler, ChangeCharacterLevelProfileHandler>();
            Register<IEventHandler, ChangeCharacterShardProfileHandler>();
            Register<IEventHandler, ChangeCharacterStarsProfileHandler>();
            Register<IEventHandler, ChangeCharacterSkillLevelProfileHandler>();
            Register<IEventHandler, ChangeRoomLevelProfileHandler>();
            Register<IEventHandler, CreateQuestProfileHandler>();
            Register<IEventHandler, ChangeQuestProgressProfileHandler>();
            Register<IEventHandler, ChangeQuestClaimedProfileHandler>();
        }
    }
}
