using System;
using System.Collections.Generic;

namespace vikwhite
{
    [Serializable]
    public class CharacterData
    {
        public string ID;
        public int Level;
        public int Shards;
        public int Stars;
        public List<SkillData> Skills = new();
    }
}
