using UnityEngine.Events;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class SummonItemViewModel : WindowViewModel<SummonItem>
    {
        private readonly IBankService _bank;
        private readonly IRewardFactory _rewardFactory;
        private readonly IRewardsWindow _rewardsWindow;

        public string Title;
        public string Count;
        public string BuyX1Text;
        public string BuyX10Text;
        public int PriceX1;
        public int PriceX10;
        public string PriceX1Text;
        public string PriceX10Text;
        public Sprite CurrencyIcon;
        public UnityAction OnBuyX1;
        public UnityAction OnBuyX10;

        public SummonItemViewModel(SummonItem model, IBankService bank, IRewardFactory rewardFactory, IRewardsWindow rewardsWindow, IConfigs configs) : base(model)
        {
            _bank = bank;
            _rewardFactory = rewardFactory;
            _rewardsWindow = rewardsWindow;

            Title = model.Name;
            BuyX1Text = "Recruit x1";
            BuyX10Text = "Recruit x10";
            PriceX1 = model.Price;
            PriceX10 = model.Price * 10;
            PriceX1Text = FormatPrice(PriceX1);
            PriceX10Text = FormatPrice(PriceX10);
            if (configs.UI.ResourceIcons.TryGetValue(model.Currency, out var icon)) CurrencyIcon = icon;

            OnBuyX1 = () => Buy(1);
            OnBuyX10 = () => Buy(10);
        }

        private string FormatPrice(int price) => price > 0 ? $"x{price}" : "Free";

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
