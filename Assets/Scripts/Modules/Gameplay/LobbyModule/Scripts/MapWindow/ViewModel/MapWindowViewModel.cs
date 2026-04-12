using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public class MapWindowViewModel: WindowViewModel<bool>
    {
        public List<MapItemViewModel> MapItems;
        
        public MapWindowViewModel(bool model, IConfigs configs) : base(model)
        {
            MapItems = new();
            foreach (var location in configs.Map.GetAll())
            {
                MapItems.Add(CreateViewModel<MapItemViewModel, string>(location.LocationID));
            }
        }
    }
}