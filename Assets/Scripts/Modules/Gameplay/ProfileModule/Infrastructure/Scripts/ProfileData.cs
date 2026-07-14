using System;
using System.Collections.Generic;

namespace vikwhite
{
    [Serializable]
    public class ProfileData
    {
        public List<CharacterData> Characters = new();
        public List<ResourceData> Resources = new();
        public List<QuestProfileData> Quests = new();
        public List<RoomData> Rooms = new();
        public string RoadMapLocation;
        public bool AutoUseSkills;
    }
}
