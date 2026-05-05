namespace vikwhite
{
    public class RewardModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IRewardFactory, RewardFactory>();
            Register<IRewardService, RewardService>();

            Register<IRewardItemViewFactory, RewardItemViewFactory>();
            Register<RewardItemViewModel>();
            Register<RewardItemView>();
        }
    }
}
