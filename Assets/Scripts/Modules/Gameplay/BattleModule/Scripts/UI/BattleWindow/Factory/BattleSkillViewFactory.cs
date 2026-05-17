namespace vikwhite
{
    public interface IBattleSkillViewFactory : IPooledViewFactory<BattleSkillView, BattleSkillViewModel> { }

    public class BattleSkillViewFactory : PooledViewFactory<BattleSkillView, BattleSkillViewModel>, IBattleSkillViewFactory
    {
        public override string AssetName => "UI/Prefabs/BattleWindow/Skill";
    }
}
