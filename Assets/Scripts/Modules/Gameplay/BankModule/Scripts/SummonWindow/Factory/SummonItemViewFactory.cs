namespace vikwhite
{
    public interface ISummonItemViewFactory : IPooledViewFactory<SummonItemView, SummonItemViewModel> { }

    public class SummonItemViewFactory : PooledViewFactory<SummonItemView, SummonItemViewModel>, ISummonItemViewFactory
    {
        public override string AssetName => "UI/Prefabs/BankWindow/SummonItem";
    }
}
