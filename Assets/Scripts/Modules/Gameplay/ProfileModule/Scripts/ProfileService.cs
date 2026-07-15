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
        private bool _autoUseSkillsChanged;
        public ProfileData Data { get; private set; } = new();
        
        public ProfileService(IConfigs configs)
        {
            _configs = configs;
        }

        public void Rest()
        {
            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
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
                    new ResourceData{ Type = ResourceType.Gold, Amount = 100 },
                },
                Quests = new (),
                Rooms = new (),
                RoadMapLocation = _configs.Map.GetAll().Where(e => e.Sector != "").First().ID,
                AutoUseSkills = false
            };
            
            foreach (var roomData in _configs.Rooms.GetAll().Where(e => e.Level == 1))
                Data.Rooms.Add(new RoomData
                {
                    Type = roomData.Type,
                    Level = 0,
                    Production = 0f,
                    Capacity = 0f,
                    LastProductionCollectionUnixTime = currentUnixTime
                });

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
            PreserveAutoUseSkillsIfUnchanged();
            string json = JsonUtility.ToJson(Data);
            File.WriteAllText(Application.persistentDataPath + "/Profile.json", json);
            _autoUseSkillsChanged = false;
        }

        public void SetAutoUseSkills(bool value)
        {
            Data.AutoUseSkills = value;
            _autoUseSkillsChanged = true;
        }
        
        public void Load()
        {
            Rest();
            string path = Application.persistentDataPath + "/Profile.json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Data = JsonUtility.FromJson<ProfileData>(json);
                if (InitializeMissingRoomProductionTimes()) Save();
            }
            else
            {
                Save();
            }

            _autoUseSkillsChanged = false;
        }

        private bool InitializeMissingRoomProductionTimes()
        {
            var changed = false;
            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (var room in Data.Rooms)
            {
                if (room.LastProductionCollectionUnixTime > 0) continue;

                room.LastProductionCollectionUnixTime = currentUnixTime;
                changed = true;
            }

            return changed;
        }

        private void PreserveAutoUseSkillsIfUnchanged()
        {
            if (_autoUseSkillsChanged) return;

            string path = Application.persistentDataPath + "/Profile.json";
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var savedData = JsonUtility.FromJson<ProfileData>(json);
            if (savedData == null) return;

            Data.AutoUseSkills = savedData.AutoUseSkills;
        }
    }
}
