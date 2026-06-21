using UniRx;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class MetaItemViewModel : WindowViewModel<Character>
    {
        public IReadOnlyReactiveProperty<int> Level { get; }
        public MetaStarsViewModel Stars { get; }
        public Sprite ClassIcon { get; }
        public Sprite RarityBackground { get; }
        public GameObject ImagePrefab { get; }

        public MetaItemViewModel(Character character, IConfigs configs) : base(character)
        {
            Level = character.Level;
            Stars = CreateViewModel<MetaStarsViewModel, IReadOnlyReactiveProperty<int>>(character.Stars);
            ImagePrefab = character.Config.ImagePrefab;
            
            configs.ClassIcons.TryGetValue(character.Config.Class, out var classIcon);
            ClassIcon = classIcon;
            
            configs.MetaRarityBG.TryGetValue(character.Config.Rarity, out var rarityBackground);
            RarityBackground = rarityBackground;
        }
    }
}
