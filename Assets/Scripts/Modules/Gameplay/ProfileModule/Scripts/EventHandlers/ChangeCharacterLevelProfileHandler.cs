namespace vikwhite
{
    public class ChangeCharacterLevelProfileHandler : ProfileHandler<ChangeCharacterLevelEvent>
    {
        protected override void Handle(ChangeCharacterLevelEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Characters.Count; i++)
            {
                if (_profile.Data.Characters[i].ID != evnt.ID) continue;
                _profile.Data.Characters[i].Level = evnt.Level;
                break;
            }
            _profile.Save();
        }
    }
}