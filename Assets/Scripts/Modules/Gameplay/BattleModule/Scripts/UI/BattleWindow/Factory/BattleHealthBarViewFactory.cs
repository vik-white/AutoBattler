namespace vikwhite
{
    public interface IBattleHealthBarViewFactory : IPooledViewFactory<BattleHealthBarView, BattleHealthBarViewModel> { }

    public class BattleHealthBarViewFactory : PooledViewFactory<BattleHealthBarView, BattleHealthBarViewModel>, IBattleHealthBarViewFactory
    {
        public override string AssetName => "UI/Prefabs/BattleWindow/HealthBar";
    }
}
