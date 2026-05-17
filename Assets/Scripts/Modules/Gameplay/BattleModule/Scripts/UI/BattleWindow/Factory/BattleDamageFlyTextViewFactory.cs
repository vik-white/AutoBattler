namespace vikwhite
{
    public interface IBattleDamageFlyTextViewFactory : IPooledViewFactory<BattleDamageFlyTextView, BattleDamageFlyTextViewModel> { }

    public class BattleDamageFlyTextViewFactory : PooledViewFactory<BattleDamageFlyTextView, BattleDamageFlyTextViewModel>, IBattleDamageFlyTextViewFactory
    {
        public override string AssetName => "UI/Prefabs/BattleWindow/DamageFlyText";
    }
}
