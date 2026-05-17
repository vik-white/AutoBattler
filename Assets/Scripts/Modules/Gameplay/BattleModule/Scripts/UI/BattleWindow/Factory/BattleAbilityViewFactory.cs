namespace vikwhite
{
    public interface IBattleAbilityViewFactory : IPooledViewFactory<BattleAbilityView, BattleAbilityViewModel> { }

    public class BattleAbilityViewFactory : PooledViewFactory<BattleAbilityView, BattleAbilityViewModel>, IBattleAbilityViewFactory
    {
        public override string AssetName => "UI/Prefabs/BattleWindow/Ability";
    }
}
