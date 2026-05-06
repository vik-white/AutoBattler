using UnityEngine.Events;

namespace vikwhite
{
    public class SummonItemViewModel : WindowViewModel<SummonItem>
    {
        private readonly IBankService _bank;
        private readonly IRewardFactory _rewardFactory;
        private readonly IRewardsWindow _rewardsWindow;

        public string Title;
        public int PriceX1;
        public int PriceX10;
        public ResourceViewModel Resource;
        public UnityAction OnBuyX1;
        public UnityAction OnBuyX10;

        public SummonItemViewModel(SummonItem model, IBankService bank, IResourceService resources, IRewardFactory rewardFactory, IRewardsWindow rewardsWindow) : base(model)
        {
            _bank = bank;
            _rewardFactory = rewardFactory;
            _rewardsWindow = rewardsWindow;
            Title = model.Name;
            PriceX1 = model.Price;
            PriceX10 = model.Price * 10;
            Resource = CreateViewModel<ResourceViewModel, Resource>(resources.Get(model.Currency));
            OnBuyX1 = () => Buy(1);
            OnBuyX10 = () => Buy(10);
        }

        private void Buy(int count)
        {
            if (_bank.TryBuy(Model, count) == false) return;
            var rewards = _rewardFactory.Create(Model.Reward, count);
            _rewardsWindow.ShowWindow(rewards);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnBuyX1 = null;
            OnBuyX10 = null;
        }
    }
}
