using System.IO;
using UnityEngine;

namespace vikwhite
{
    public class ProfileService : IProfileService
    {
        public ProfileData Data { get; private set; } = new();

        public void Rest()
        {
            Data = new ProfileData()
            {
                Resources =
                {
                    new ResourceData{ Type = ResourceType.Hard, Amount = 100 },
                    new ResourceData{ Type = ResourceType.Soft, Amount = 300 }, 
                },
                Squad = new [] {"","","","",""}
            };
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