namespace vikwhite
{
    public class QuestsModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IQuestFactory, QuestFactory>();
            Register<IQuestsService, QuestsService>();

            Register<IQuestItemViewFactory, QuestItemViewFactory>();
            Register<QuestItemViewModel>();
            Register<QuestItemView>();
        }
    }
}
