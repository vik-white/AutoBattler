namespace vikwhite
{
    public class ChangeCharacterSkillLevelEvent
    {
        public string ID;
        public string SkillID;
        public int SkillLevel;

        public ChangeCharacterSkillLevelEvent(string id, string skillID, int skillLevel)
        {
            ID = id;
            SkillID = skillID;
            SkillLevel = skillLevel;
        }
    }
}
