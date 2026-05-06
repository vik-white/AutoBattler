namespace vikwhite
{
    public class ChangeCharacterSkillLevelProfileHandler : ProfileHandler<ChangeCharacterSkillLevelEvent>
    {
        protected override void Handle(ChangeCharacterSkillLevelEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Characters.Count; i++)
            {
                if (_profile.Data.Characters[i].ID != evnt.ID) continue;
                _profile.Data.Characters[i].SkillLevel = evnt.SkillLevel;
                break;
            }
            _profile.Save();
        }
    }
}
