namespace vikwhite
{
    public interface IQuestItemViewFactory : IPooledViewFactory<QuestItemView, QuestItemViewModel> { }

    public class QuestItemViewFactory : PooledViewFactory<QuestItemView, QuestItemViewModel>, IQuestItemViewFactory
    {
        public override string AssetName => "UI/Prefabs/Elements/QuestItem";
    }
}
