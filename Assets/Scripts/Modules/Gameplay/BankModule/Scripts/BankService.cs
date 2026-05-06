using System.Collections.Generic;
using vikwhite.Data;

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

        public BankService(IConfigs configs, IResourceService resources)
        {
            _resources = resources;
            _summonItems = new List<SummonItem>();

            foreach (var data in configs.Summons.GetAll())
            {
                _summonItems.Add(new SummonItem
                {
                    Name = data.Name,
                    Currency = data.Resource,
                    Price = data.Price,
                    Reward = data.Reward,
                });
            }
        }

        public bool TryBuy(SummonItem item, int count)
        {
            if (item == null || count <= 0) return false;

            var price = count >= 10 ? item.Price * 10 : item.Price;
            if (_resources.GetAmount(item.Currency).Value < price) return false;

            _resources.Spend(item.Currency, price);
            return true;
        }
    }
}
