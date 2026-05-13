using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class CardViewModel: WindowViewModel<Character>
    {
        public UnityAction OnSelect;
        public string ID { get; }
        public Color RarityColor { get; }
        public IReadOnlyReactiveProperty<int> Level;
        
        public CardViewModel(Character model, ICharacterWindow characterWindow, IConfigs configs) : base(model)
        {
            ID = model.ID;
            Level = model.Level;
            OnSelect = () => characterWindow.ShowWindow(model);
            RarityColor = configs.RarityColors[model.Config.Rarity];
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}