namespace vikwhite
{
    public interface IRewardItemViewFactory : IPooledViewFactory<RewardItemView, RewardItemViewModel> { }

    public class RewardItemViewFactory : PooledViewFactory<RewardItemView, RewardItemViewModel>, IRewardItemViewFactory
    {
        public override string AssetName => "UI/Prefabs/Elements/RewardItem";
    }
}
