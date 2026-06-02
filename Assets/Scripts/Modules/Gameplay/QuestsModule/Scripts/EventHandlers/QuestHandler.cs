namespace vikwhite
{
    public class QuestHandler<T> : EventHandler<T>
    {
        protected readonly IQuestRegistry _registry = DI.Resolve<IQuestRegistry>();

        protected override void Handle(T evnt) { }
    }
}
