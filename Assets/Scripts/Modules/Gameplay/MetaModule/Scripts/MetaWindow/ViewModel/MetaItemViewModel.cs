using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class MetaItemViewModel : WindowViewModel<Character>
    {
        public UnityAction OnSelect;
        public IReadOnlyReactiveProperty<int> Level { get; }
        public MetaStarsViewModel Stars { get; }
        public Sprite ClassIcon { get; }
        public Sprite RarityBackground { get; }
        public GameObject ImagePrefab { get; }

        public MetaItemViewModel(Character character, ICharacterWindow characterWindow, IConfigs configs) : base(character)
        {
            OnSelect = () => characterWindow.ShowWindow(character);
            Level = character.Level;
            Stars = CreateViewModel<MetaStarsViewModel, IReadOnlyReactiveProperty<int>>(character.Stars);
            ImagePrefab = character.Config.HeadPrefab;
            
            configs.UI.ClassIcons.TryGetValue(character.Config.Class, out var classIcon);
            ClassIcon = classIcon;
            
            configs.UI.Rarities.TryGetValue(character.Config.Rarity, out var rarity);
            RarityBackground = rarity.MetaBG;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}
