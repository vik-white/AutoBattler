using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadItemViewModel: WindowViewModel<Character>
    {
        public UnityAction OnSelect;
        public IReadOnlyReactiveProperty<int> Level;
        public Sprite RarityBG { get; }
        public Sprite ClassIcon { get; }
        public GameObject ImagePrefab { get; }
        
        public SquadItemViewModel(Character model, IConfigs configs) : base(model)
        {
            Level = model.Level;
            ImagePrefab = model.Config.HeadPrefab;
            ClassIcon = configs.UI.ClassIcons[model.Config.Class];
            RarityBG = configs.UI.Rarities[model.Config.Rarity].MetaBG;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}