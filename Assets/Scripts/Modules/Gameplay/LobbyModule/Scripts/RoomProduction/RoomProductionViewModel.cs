using UniRx;
using UnityEngine;

namespace vikwhite
{
    public class RoomProductionViewModel : ViewModel<RoomProductionModel>
    {
        public ResourceType Type => Model.Type;
        public IReadOnlyReactiveProperty<float> Production => Model.Room.Production;

        public RoomProductionViewModel(RoomProductionModel model) : base(model) { }

        public bool TryGetWorldPosition(out Vector3 position)
        {
            if (Model.Anchor == null)
            {
                position = default;
                return false;
            }

            position = Model.Anchor.bounds.center;
            return true;
        }
    }
}
