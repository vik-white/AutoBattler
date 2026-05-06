namespace vikwhite
{
    public class ChangeCharacterStarsProfileHandler : ProfileHandler<ChangeCharacterStarsEvent>
    {
        protected override void Handle(ChangeCharacterStarsEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Characters.Count; i++)
            {
                if (_profile.Data.Characters[i].ID != evnt.ID) continue;
                _profile.Data.Characters[i].Stars = evnt.Stars;
                break;
            }
            _profile.Save();
        }
    }
}
