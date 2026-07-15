using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CheatWindowViewModel: WindowViewModel
    {
        public List<MapItemViewModel> MapItems;
        public UnityAction OnAddGem;
        public UnityAction OnAddExp;
        public UnityAction OnAddBook;
        public UnityAction OnAddKeyCommon;
        public UnityAction OnAddKeyEpic;
        public UnityAction OnAddGold;
        
        public CheatWindowViewModel(IConfigs configs, IResourceService resource)
        {
            MapItems = new();
            foreach (var location in configs.Map.GetAll())
            {
                MapItems.Add(CreateViewModel<MapItemViewModel, IMapData>(location));
            }
            OnAddGem = () => resource.Add(ResourceType.Gem, 100);
            OnAddExp = () => resource.Add(ResourceType.Exp, 100);
            OnAddBook = () => resource.Add(ResourceType.Book, 10);
            OnAddKeyCommon = () => resource.Add(ResourceType.KeyCommon, 10);
            OnAddKeyEpic = () => resource.Add(ResourceType.KeyEpic, 10);
            OnAddGold = () => resource.Add(ResourceType.Gold, 100);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnAddGem = null;
            OnAddGold = null;
            OnAddBook = null;
            OnAddKeyCommon = null;
            OnAddKeyEpic = null;
            OnAddGold = null;
        }
    }
}
