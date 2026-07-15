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
}
