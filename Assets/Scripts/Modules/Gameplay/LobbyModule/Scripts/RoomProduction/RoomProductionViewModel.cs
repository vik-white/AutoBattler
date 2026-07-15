using System;
using UnityEngine;
using UnityEngine.Events;

namespace vikwhite
{
    public class RoomProductionViewModel : ViewModel<RoomProductionModel>
    {
        private readonly IRoomsService _roomsService;

        public ResourceType Type => Model.Type;
        public bool HasProduction => Model.Room.Production.Value > 0f;
        public UnityAction OnCollect;

        public RoomProductionViewModel(RoomProductionModel model, IRoomsService roomsService) : base(model)
        {
            _roomsService = roomsService;
            OnCollect = Collect;
        }

        public RoomProductionState GetState()
        {
            return RoomProductionCalculator.Calculate(
                Model.Room.LastProductionCollectionUnixTime.Value,
                Model.Room.Production.Value,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

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

        private void Collect() => _roomsService.CollectProduction(Model.Room, Model.Type);

        public override void Dispose()
        {
            base.Dispose();
            OnCollect = null;
        }
    }
}
