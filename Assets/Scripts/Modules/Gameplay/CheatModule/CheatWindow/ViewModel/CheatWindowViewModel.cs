using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public class CheatWindowViewModel: WindowViewModel<bool>
    {
        public List<MapItemViewModel> MapItems;
        
        public CheatWindowViewModel(bool model, IConfigs configs) : base(model)
        {
            MapItems = new();
            foreach (var location in configs.Map.GetAll())
            {
                MapItems.Add(CreateViewModel<MapItemViewModel, IMapData>(location));
            }
        }
    }
}