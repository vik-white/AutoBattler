namespace vikwhite
{
    public class RewardModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IRewardService, RewardService>();
        }
    }
}
