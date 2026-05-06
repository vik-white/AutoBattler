using System.Collections.Generic;

namespace vikwhite
{
    public interface IBankService
    {
        IReadOnlyList<SummonItem> SummonItems { get; }
        bool TryBuy(SummonItem item, int count);
    }

    public class BankService : IBankService
    {
        private readonly IResourceService _resources;
        private readonly List<SummonItem> _summonItems;

        public IReadOnlyList<SummonItem> SummonItems => _summonItems;

        public BankService(IResourceService resources)
        {
            _resources = resources;
            // TODO: move to Configs when summon table is ready
            _summonItems = new List<SummonItem>
            {
                new SummonItem { Name = "Common Summon",  Currency = ResourceType.Gold, PriceX1 = 100, PriceX10 = 900 },
                new SummonItem { Name = "Premium Summon", Currency = ResourceType.Gem,  PriceX1 = 10,  PriceX10 = 90  },
            };
        }

        public bool TryBuy(SummonItem item, int count)
        {
            if (item == null || count <= 0) return false;

            var price = count >= 10 ? item.PriceX10 : item.PriceX1;
            if (_resources.GetAmount(item.Currency).Value < price) return false;

            _resources.Spend(item.Currency, price);
            return true;
        }
    }
}
