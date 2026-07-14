using UniRx;
using UnityEngine.Events;

namespace vikwhite
{
    public class RoomWindowViewModel : WindowViewModel<Room>
    {
        public string Title => Model.Type.ToString();
        public IReadOnlyReactiveProperty<int> Level => Model.Level;
        public UnityAction OnUpgrade;

        public RoomWindowViewModel(Room room) : base(room)
        {
            OnUpgrade = Model.Upgrade;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnUpgrade = null;
        }
    }
}
