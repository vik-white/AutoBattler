namespace vikwhite
{
    public class ChangeRoomLevelProfileHandler : ProfileHandler<ChangeRoomLevelEvent>
    {
        protected override void Handle(ChangeRoomLevelEvent evnt)
        {
            foreach (var roomData in _profile.Data.Rooms)
            {
                if (roomData.Type != evnt.Type) continue;
                roomData.Level = evnt.Level;
                _profile.Save();
                return;
            }
        }
    }

    public class ChangeRoomProductionProfileHandler : ProfileHandler<ChangeRoomProductionEvent>
    {
        protected override void Handle(ChangeRoomProductionEvent evnt)
        {
            foreach (var roomData in _profile.Data.Rooms)
            {
                if (roomData.Type != evnt.Type) continue;
                roomData.Production = evnt.Production;
                _profile.Save();
                return;
            }
        }
    }

    public class ChangeRoomCapacityProfileHandler : ProfileHandler<ChangeRoomCapacityEvent>
    {
        protected override void Handle(ChangeRoomCapacityEvent evnt)
        {
            foreach (var roomData in _profile.Data.Rooms)
            {
                if (roomData.Type != evnt.Type) continue;
                roomData.Capacity = evnt.Capacity;
                _profile.Save();
                return;
            }
        }
    }

    public class ChangeRoomProductionCollectionTimeProfileHandler
        : ProfileHandler<ChangeRoomProductionCollectionTimeEvent>
    {
        protected override void Handle(ChangeRoomProductionCollectionTimeEvent evnt)
        {
            foreach (var roomData in _profile.Data.Rooms)
            {
                if (roomData.Type != evnt.Type) continue;
                roomData.LastProductionCollectionUnixTime = evnt.UnixTime;
                _profile.Save();
                return;
            }
        }
    }

    public class ChangeRoomUpgradeStartTimeProfileHandler : ProfileHandler<ChangeRoomUpgradeStartTimeEvent>
    {
        protected override void Handle(ChangeRoomUpgradeStartTimeEvent evnt)
        {
            foreach (var roomData in _profile.Data.Rooms)
            {
                if (roomData.Type != evnt.Type) continue;
                roomData.UpgradeStartUnixTime = evnt.UnixTime;
                _profile.Save();
                return;
            }
        }
    }
}
