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
                    new ResourceData{ Type = ResourceType.Gold, Amount = 0 },
                    new ResourceData{ Type = ResourceType.Book, Amount = 0 },
                    new ResourceData{ Type = ResourceType.KeyCommon, Amount = 0 },
                    new ResourceData{ Type = ResourceType.KeyEpic, Amount = 0 },
                },
                ClassShards = new (),
                ClassBooks = new (),
                Squad = new [] {"","","","",""}
            };

            foreach (CharacterClassType @class in Enum.GetValues(typeof(CharacterClassType)))
            foreach (RarityType rarity in Enum.GetValues(typeof(RarityType)))
                Data.ClassShards.Add(new ClassShardData { Class = @class, Rarity = rarity, Amount = 0 });

            foreach (CharacterClassType @class in Enum.GetValues(typeof(CharacterClassType)))
                Data.ClassBooks.Add(new ClassBookData { Class = @class, Amount = 0 });

            foreach (var characterData in _configs.Characters.GetAll())
            {
                if (characterData.Squad)
                    Data.Characters.Add(new CharacterData { ID = characterData.ID, Level = 1, Shards = 0, Stars = 0, SkillLevel = 1 } );
            }
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
                Migrate();
            }
            else
            {
                Save();
            }
        }

        private void Migrate()
        {
            Data.Resources ??= new List<ResourceData>();
            Data.Characters ??= new List<CharacterData>();
            Data.ClassShards ??= new List<ClassShardData>();
            Data.ClassBooks ??= new List<ClassBookData>();

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if (Data.Resources.All(r => r.Type != type))
                    Data.Resources.Add(new ResourceData { Type = type, Amount = 0 });
            }

            foreach (CharacterClassType @class in Enum.GetValues(typeof(CharacterClassType)))
            foreach (RarityType rarity in Enum.GetValues(typeof(RarityType)))
            {
                if (Data.ClassShards.All(s => s.Class != @class || s.Rarity != rarity))
                    Data.ClassShards.Add(new ClassShardData { Class = @class, Rarity = rarity, Amount = 0 });
            }

            foreach (CharacterClassType @class in Enum.GetValues(typeof(CharacterClassType)))
            {
                if (Data.ClassBooks.All(b => b.Class != @class))
                    Data.ClassBooks.Add(new ClassBookData { Class = @class, Amount = 0 });
            }
        }
    }
}