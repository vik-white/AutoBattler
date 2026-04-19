using System;
using Sirenix.OdinInspector;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

namespace vikwhite.Data
{
    public interface IConfigs
    {
        IConfig<ICharacterData> Characters { get; }
        IConfig<ISquadData> Squad { get; }
        IConfig<IMapData> Map { get; }
        IConfig<ILocationStaticData> LocationStatic { get; }
        IConfig<ILocationFlowData> LocationFlow { get; }
        IConfig<IAbilityData> Abilities { get; }
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "ConfigsLoader", menuName = "vikwhite/ConfigsLoader")]
    public class ConfigsLoader : ScriptableObject, IConfigs
    {
        public string ID = "1ZDvO0_zoEDrl4y1ueu5SGoG0xvb6Ay9yAsMuU3hQuTM";
        public string APIKey = "AIzaSyBXrlvSuX9jHyVcEAfB2NBVM1QQJQ7rPBk";
        [Space(30)]
        
        [SerializeField] private Config<CharacterData, ICharacterData> characters;
        [SerializeField] private Config<SquadData, ISquadData> squad;
        [SerializeField] private Config<MapData, IMapData> map;
        [SerializeField] private Config<LocationStaticData, ILocationStaticData> locationStatic;
        [SerializeField] private Config<LocationFlowData, ILocationFlowData> locationFlow;
        [SerializeField] private Config<AbilityData, IAbilityData> abilities;
        
        public IConfig<ICharacterData> Characters => characters;
        public IConfig<ISquadData> Squad => squad;
        public IConfig<IMapData> Map => map;
        public IConfig<ILocationStaticData> LocationStatic => locationStatic;
        public IConfig<ILocationFlowData> LocationFlow => locationFlow;
        public IConfig<IAbilityData> Abilities => abilities;
        
        #if UNITY_EDITOR
        [Button("Load")][PropertyOrder(-1)]
        private void Load() {
            int configLoadedCount = 0;
            foreach(FieldInfo field in ConfigCore.Fields) {
                if(!field.FieldType.Equals(typeof(string))) {
                    EditorCoroutineUtility.StartCoroutine((field.GetValue(this) as ConfigCore).Load(field.Name.CapitalizeFirstLetter(), ID, APIKey, (count) => {
                        Debug.Log(field.Name + " - " + count + " - LOAD COMPLETED!");
                        configLoadedCount++;
                        if(configLoadedCount == ConfigCore.Fields.Length) {
                            ConfigCore.Fields.ToList().ForEach(e => (e.GetValue(this) as ConfigCore).ConnectParseField(this, e));
                            ConfigCore.Fields.ToList().ForEach(e => (e.GetValue(this) as ConfigCore).ConnectData(this));
                        }
                    }), this);
                }
            }
            EditorUtility.SetDirty(this);
        }
        #endif
    }
}