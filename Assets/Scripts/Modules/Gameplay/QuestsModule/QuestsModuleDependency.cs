namespace vikwhite
{
    public class QuestsModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IQuestRegistry, QuestRegistry>();
            Register<IQuestFactory, QuestFactory>();
            Register<Quest>();

            Register<IEventHandler, SetSectorLocationQuestHandler>();
            Register<IEventHandler, ChangeCharacterLevelQuestHandler>();
            Register<IEventHandler, ChangeResourceQuestHandler>();

            Register<IQuestItemViewFactory, QuestItemViewFactory>();
            Register<QuestItemViewModel>();
            Register<QuestItemView>();
        }
    }
}
