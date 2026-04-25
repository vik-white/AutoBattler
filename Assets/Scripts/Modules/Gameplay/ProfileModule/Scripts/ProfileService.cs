using System.IO;
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
                    new ResourceData{ Type = ResourceType.Hard, Amount = 100 },
                    new ResourceData{ Type = ResourceType.Soft, Amount = 300 }, 
                },
                Squad = new [] {"","","","",""}
            };

            foreach (var characterData in _configs.Characters.GetAll())
            {
                if (characterData.Squad)
                    Data.Characters.Add(new CharacterData { ID = characterData.ID, Level = 0 } );
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
            }
            else
            {
                Save();
            }
        }
    }
}