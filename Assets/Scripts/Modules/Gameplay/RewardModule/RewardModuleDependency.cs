namespace vikwhite
{
    public class RewardModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IRewardFactory, RewardFactory>();
            Register<IRewardService, RewardService>();
        }
    }
}
