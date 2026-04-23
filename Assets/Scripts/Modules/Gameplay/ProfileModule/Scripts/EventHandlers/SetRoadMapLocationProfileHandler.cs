namespace vikwhite
{
    public class SetRoadMapLocationProfileHandler : ProfileHandler<SetRoadMapLocationEvent>
    {
        protected override void Handle(SetRoadMapLocationEvent evnt)
        {
            _profile.Data.RoadMapLocation = evnt.ID;
            _profile.Save();
        }
    }
}