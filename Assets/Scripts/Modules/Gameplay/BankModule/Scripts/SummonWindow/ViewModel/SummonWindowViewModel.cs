using System.Collections.Generic;

namespace vikwhite
{
    public class SummonWindowViewModel : WindowViewModel
    {
        public List<SummonItemViewModel> SummonItems = new();
        public List<ResourceViewModel> Resources = new ();

        public SummonWindowViewModel(IBankService bank, IResourceService resource)
        {
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.KeyCommon)));
            Resources.Add(CreateViewModel<ResourceViewModel, Resource>(resource.Get(ResourceType.KeyEpic)));
            foreach (var item in bank.SummonItems)
                SummonItems.Add(CreateViewModel<SummonItemViewModel, SummonItem>(item));
        }
    }
}
