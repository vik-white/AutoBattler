namespace vikwhite
{
    public class ChangeCharacterSkillLevelProfileHandler : ProfileHandler<ChangeCharacterSkillLevelEvent>
    {
        protected override void Handle(ChangeCharacterSkillLevelEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Characters.Count; i++)
            {
                if(_profile.Data.Characters[i].ID != evnt.ID) continue;
                for (int s = 0; s < _profile.Data.Characters[i].Skills.Count; s++)
                {
                    if (_profile.Data.Characters[i].Skills[s].ID != evnt.SkillID) continue; 
                    _profile.Data.Characters[i].Skills[s].Level = evnt.SkillLevel;
                    _profile.Save();
                    return;
                }
            }
        }
    }
}