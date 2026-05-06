using System.Collections.Generic;

namespace vikwhite
{
    public class SummonWindowViewModel : WindowViewModel
    {
        public List<SummonItemViewModel> SummonItems = new();

        public SummonWindowViewModel(IBankService bank)
        {
            foreach (var item in bank.SummonItems)
                SummonItems.Add(CreateViewModel<SummonItemViewModel, SummonItem>(item));
        }
    }
}
