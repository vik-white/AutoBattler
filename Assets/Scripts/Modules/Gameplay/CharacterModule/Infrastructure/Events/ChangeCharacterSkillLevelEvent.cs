namespace vikwhite
{
    public class ChangeCharacterSkillLevelEvent
    {
        public string ID;
        public int SkillLevel;

        public ChangeCharacterSkillLevelEvent(string id, int skillLevel)
        {
            ID = id;
            SkillLevel = skillLevel;
        }
    }
}
