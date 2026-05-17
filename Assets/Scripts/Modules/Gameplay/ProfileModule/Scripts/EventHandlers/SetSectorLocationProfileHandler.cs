namespace vikwhite
{
    public class SetSectorLocationProfileHandler : ProfileHandler<SetSectorLocationEvent>
    {
        protected override void Handle(SetSectorLocationEvent evnt)
        {
            _profile.Data.RoadMapLocation = evnt.ID;
            _profile.Save();
        }
    }
}
