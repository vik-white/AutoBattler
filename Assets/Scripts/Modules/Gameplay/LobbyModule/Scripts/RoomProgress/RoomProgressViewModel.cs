using UnityEngine;

namespace vikwhite
{
    public class RoomProgressViewModel : ViewModel<RoomProgressModel>
    {
        private readonly IRoomsService _roomsService;

        public RoomType Type => Model.Room.Type;

        public RoomProgressViewModel(RoomProgressModel model, IRoomsService roomsService) : base(model)
        {
            _roomsService = roomsService;
        }

        public RoomUpgradeState GetState() => _roomsService.GetUpgradeState(Model.Room);

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
