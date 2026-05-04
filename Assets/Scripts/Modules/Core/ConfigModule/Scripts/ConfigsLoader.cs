using System;
using System.Collections.Generic;
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
        IConfig<IMapData> Map { get; }
        IConfig<ILocationStaticData> LocationStatic { get; }
        IConfig<ILocationFlowData> LocationFlow { get; }
        IConfig<IAbilityData> Abilities { get; }
        IConfig<IHexPositionsData> HexPositions { get; }
        IConfig<ILevelUpData> LevelUp { get; }
        IConfig<IRewardsData> Rewards { get; }
        
        IReadOnlyDictionary<ResourceType, Sprite> ResourceIcons { get; }
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "ConfigsLoader", menuName = "vikwhite/ConfigsLoader")]
    public class ConfigsLoader : SerializedScriptableObject, IConfigs
    {
        public string ID = "1ZDvO0_zoEDrl4y1ueu5SGoG0xvb6Ay9yAsMuU3hQuTM";
        public string APIKey = "AIzaSyBXrlvSuX9jHyVcEAfB2NBVM1QQJQ7rPBk";
        [Space(30)] 
        
        [SerializeField] private Config<CharacterData, ICharacterData> characters;
        [SerializeField] private Config<MapData, IMapData> map;
        [SerializeField] private Config<LocationStaticData, ILocationStaticData> locationStatic;
        [SerializeField] private Config<LocationFlowData, ILocationFlowData> locationFlow;
        [SerializeField] private Config<AbilityData, IAbilityData> abilities;
        [SerializeField] private Config<HexPositionsData, IHexPositionsData> hexPositions;
        [SerializeField] private Config<LevelUpData, ILevelUpData> levelUp;
        [SerializeField] private Config<RewardsData, IRewardsData> rewards;
        
        [TableList][SerializeField] List<ResourceIconData> resourceIcons;
        private Dictionary<ResourceType, Sprite> resourceIconsDictionary;
        
        public IConfig<ICharacterData> Characters => characters;
        public IConfig<IMapData> Map => map;
        public IConfig<ILocationStaticData> LocationStatic => locationStatic; 
        public IConfig<ILocationFlowData> LocationFlow => locationFlow;
        public IConfig<IAbilityData> Abilities => abilities;
        public IConfig<IHexPositionsData> HexPositions => hexPositions;
        public IConfig<ILevelUpData> LevelUp => levelUp;
        public IConfig<IRewardsData> Rewards => rewards;
        
        public IReadOnlyDictionary<ResourceType, Sprite> ResourceIcons
        {
            get
            {
                if (resourceIconsDictionary == null)
                {
                    resourceIconsDictionary = new Dictionary<ResourceType, Sprite>();
                    foreach (var resource in resourceIcons)
                        resourceIconsDictionary.Add(resource.Type, resource.Icon);
                }
                return resourceIconsDictionary;
            }
        }

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