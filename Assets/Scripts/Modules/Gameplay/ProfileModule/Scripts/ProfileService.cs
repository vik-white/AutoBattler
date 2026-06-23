using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class ProfileService : IProfileService
    {
        private readonly IConfigs _configs;
        public ProfileData Data { get; private set; } = new();
        
        public ProfileService(IConfigs configs)
        {
            _configs = configs;
        }

        public void Rest()
        {
            Data = new ProfileData()
            {
                Characters = new (),
                Resources =
                {
                    new ResourceData{ Type = ResourceType.Gem, Amount = 0 },
                    new ResourceData{ Type = ResourceType.Exp, Amount = 0 },
                    new ResourceData{ Type = ResourceType.Book, Amount = 0 },
                    new ResourceData{ Type = ResourceType.KeyCommon, Amount = 0 },
                    new ResourceData{ Type = ResourceType.KeyEpic, Amount = 0 },
                    new ResourceData{ Type = ResourceType.ShardRare, Amount = 0 },
                    new ResourceData{ Type = ResourceType.ShardEpic, Amount = 0 },
                    new ResourceData{ Type = ResourceType.ShardLegendary, Amount = 0 },
                    new ResourceData{ Type = ResourceType.BookAssassin, Amount = 0 },
                    new ResourceData{ Type = ResourceType.BookMage, Amount = 0 },
                    new ResourceData{ Type = ResourceType.BookMystic, Amount = 0 },
                    new ResourceData{ Type = ResourceType.BookSupport, Amount = 0 },
                    new ResourceData{ Type = ResourceType.BookTank, Amount = 0 },
                },
                Quests = new (),
                Squad = new [] {"","","","",""},
                RoadMapLocation = _configs.Map.GetAll().Where(e => e.Sector != "").First().ID
            };

            foreach (var characterData in _configs.Characters.GetAll())
            {
                if (characterData.Squad)
                    Data.Characters.Add(new CharacterData
                    {
                        ID = characterData.ID,
                        Level = 1,
                        Shards = 0,
                        Stars = 0,
                        Skills = CreateSkills(characterData)
                    });
            }
        }

        private List<SkillData> CreateSkills(ICharacterData characterData)
        {
            var skills = new List<SkillData>();
            foreach (var slot in SkillSlotExtensions.CharacterSlots)
            {
                var skillID = characterData.GetSkill(slot);
                if (string.IsNullOrEmpty(skillID)) continue;
                var level = _configs.Stars.Get().GetMaxSkillLevel(slot);
                skills.Add(new SkillData { ID = skillID, Level = level });
            }
            return skills;
        }
        
        public void Save()
        {
            string json = JsonUtility.ToJson(Data);
            File.WriteAllText(Application.persistentDataPath + "/Profile.json", json);
        }
        
        public void Load()
        {
            Rest();
            string path = Application.persistentDataPath + "/Profile.json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Data = JsonUtility.FromJson<ProfileData>(json);
            }
            else
            {
                Save();
            }
        }
    }
}
