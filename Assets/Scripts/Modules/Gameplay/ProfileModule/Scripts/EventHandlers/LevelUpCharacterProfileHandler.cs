namespace vikwhite
{
    public class LevelUpCharacterProfileHandler : ProfileHandler<LevelUpCharacterEvent>
    {
        protected override void Handle(LevelUpCharacterEvent evnt)
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