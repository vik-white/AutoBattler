using UnityEngine.Events;

namespace vikwhite
{
    public class SummonItemViewModel : WindowViewModel<SummonItem>
    {
        private readonly IBankService _bank;

        public string Title;
        public int PriceX1;
        public int PriceX10;
        public ResourceViewModel Resource;
        public UnityAction OnBuyX1;
        public UnityAction OnBuyX10;

        public SummonItemViewModel(SummonItem model, IBankService bank, IResourceService resources) : base(model)
        {
            _bank = bank;
            Title = model.Name;
            PriceX1 = model.PriceX1;
            PriceX10 = model.PriceX10;
            Resource = CreateViewModel<ResourceViewModel, Resource>(resources.Get(model.Currency));
            OnBuyX1 = () => _bank.TryBuy(Model, 1);
            OnBuyX10 = () => _bank.TryBuy(Model, 10);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnBuyX1 = null;
            OnBuyX10 = null;
        }
    }
}
