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
        ISettingData Settings { get; }
        IConfig<ICharacterData> Characters { get; }
        IConfig<IMapData> Map { get; }
        IConfig<ILocationStaticData> LocationStatic { get; }
        IConfig<ILocationFlowData> LocationFlow { get; }
        IConfig<ISkillData> Skills { get; }
        IConfig<IHexPositionsData> HexPositions { get; }
        IConfig<IUpgradeData> Upgrades { get; }
        IConfig<IRewardsData> Rewards { get; }
        IConfig<ISummonData> Summons { get; }
        IConfig<IStarData> Stars { get; }
        
        IReadOnlyDictionary<ResourceType, Sprite> ResourceIcons { get; }
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "ConfigsLoader", menuName = "vikwhite/ConfigsLoader")]
    public class ConfigsLoader : SerializedScriptableObject, IConfigs
    {
        public string ID = "1ZDvO0_zoEDrl4y1ueu5SGoG0xvb6Ay9yAsMuU3hQuTM";
        public string APIKey = "AIzaSyBXrlvSuX9jHyVcEAfB2NBVM1QQJQ7rPBk";
        [Space(30)] 
        
        [SerializeField] private Config<SettingData, ISettingData> settings;
        [SerializeField] private Config<CharacterData, ICharacterData> characters;
        [SerializeField] private Config<MapData, IMapData> map;
        [SerializeField] private Config<LocationStaticData, ILocationStaticData> locationStatic;
        [SerializeField] private Config<LocationFlowData, ILocationFlowData> locationFlow;
        [SerializeField] private Config<SkillData, ISkillData> skills;
        [SerializeField] private Config<HexPositionsData, IHexPositionsData> hexPositions;
        [SerializeField] private Config<UpgradeData, IUpgradeData> upgrades;
        [SerializeField] private Config<RewardsData, IRewardsData> rewards;
        [SerializeField] private Config<SummonData, ISummonData> summons;
        [SerializeField] private Config<StarData, IStarData> stars;
        
        [TableList][SerializeField] List<ResourceIconData> resourceIcons;
        private Dictionary<ResourceType, Sprite> resourceIconsDictionary;
        
        public ISettingData Settings => settings.Get();
        public IConfig<ICharacterData> Characters => characters;
        public IConfig<IMapData> Map => map;
        public IConfig<ILocationStaticData> LocationStatic => locationStatic; 
        public IConfig<ILocationFlowData> LocationFlow => locationFlow;
        public IConfig<ISkillData> Skills => skills;
        public IConfig<IHexPositionsData> HexPositions => hexPositions;
        public IConfig<IUpgradeData> Upgrades => upgrades;
        public IConfig<IRewardsData> Rewards => rewards;
        public IConfig<ISummonData> Summons => summons;
        public IConfig<IStarData> Stars => stars;
        
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