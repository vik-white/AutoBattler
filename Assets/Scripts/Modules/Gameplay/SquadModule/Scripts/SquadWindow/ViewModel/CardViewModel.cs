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
        public string Name { get; }
        public Color RarityColor { get; }
        public IReadOnlyReactiveProperty<int> Level;
        
        public CardViewModel(Character model, ICharacterWindow characterWindow, IConfigs configs) : base(model)
        {
            ID = model.ID;
            Name = model.Config.Name;
            Level = model.Level;
            OnSelect = () => characterWindow.ShowWindow(model);
            RarityColor = Color.white;
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}