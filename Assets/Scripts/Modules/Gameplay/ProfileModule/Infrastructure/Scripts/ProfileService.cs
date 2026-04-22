using System.IO;
using UnityEngine;

namespace vikwhite
{
    public interface IProfileService
    {
        ProfileData Data { get; }
        void Save();
        void Load();
    }

    public class ProfileService : IProfileService
    {
        public ProfileData Data { get; private set; } = new();

        public void Rest()
        {
            Data = new ProfileData()
            {
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
            string path = Application.persistentDataPath + "/Profile.json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                Data = JsonUtility.FromJson<ProfileData>(json);
            }
            else
            {
                Rest();
                Save();
            }
        }
    }
}