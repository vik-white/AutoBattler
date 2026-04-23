using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CheatWindowViewModel: WindowViewModel<bool>
    {
        public List<MapItemViewModel> MapItems;
        public UnityAction OnAddGem;
        public UnityAction OnAddGold;
        
        public CheatWindowViewModel(bool model, IConfigs configs, IResourceService resource) : base(model)
        {
            MapItems = new();
            foreach (var location in configs.Map.GetAll())
            {
                MapItems.Add(CreateViewModel<MapItemViewModel, IMapData>(location));
            }
            OnAddGem = () => resource.Add(ResourceType.Hard, 100);
            OnAddGold = () => resource.Add(ResourceType.Soft, 100);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnAddGem = null;
            OnAddGold = null;
        }
    }
}