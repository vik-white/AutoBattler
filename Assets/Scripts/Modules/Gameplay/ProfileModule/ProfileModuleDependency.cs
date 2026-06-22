namespace vikwhite
{
    public class ProfileModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IProfileService, ProfileService>();
            Register<IEventHandler, SetSquadCharacterProfileHandler>();
            Register<IEventHandler, SetSectorLocationProfileHandler>();
            Register<IEventHandler, ChangeResourceProfileHandler>();
            Register<IEventHandler, ChangeCharacterLevelProfileHandler>();
            Register<IEventHandler, ChangeCharacterShardProfileHandler>();
            Register<IEventHandler, ChangeCharacterStarsProfileHandler>();
            Register<IEventHandler, ChangeCharacterSkillLevelProfileHandler>();
            Register<IEventHandler, CreateQuestProfileHandler>();
            Register<IEventHandler, ChangeQuestProgressProfileHandler>();
            Register<IEventHandler, ChangeQuestClaimedProfileHandler>();
        }
    }
}
