using System.IO;
using UnityEditor;
using UnityEngine;

namespace vikwhite
{
    public static class ProfileTools
    {
        [MenuItem("Tools/Profile/Delete Profile File")]
        public static void DeleteProfile()
        {
            string path = Application.persistentDataPath + "/Profile.json";
            if (File.Exists(path)) File.Delete(path);
        }
    }
}