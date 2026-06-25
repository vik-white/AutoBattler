using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadItemViewModel: WindowViewModel<Character>
    {
        private readonly ReactiveProperty<bool> _isSelected = new(false);

        public UnityAction OnSelect;
        public IReadOnlyReactiveProperty<int> Level;
        public IReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
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

        public void SetSelected(bool isSelected)
        {
            _isSelected.Value = isSelected;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
            _isSelected.Dispose();
        }
    }
}
