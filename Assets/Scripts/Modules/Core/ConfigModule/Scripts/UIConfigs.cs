using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace vikwhite.Data
{
    public interface IUIConfigs
    {
        IReadOnlyDictionary<ResourceType, Sprite> ResourceIcons { get; }
        IReadOnlyDictionary<RarityType, RarityUIData> Rarities { get; }
        IReadOnlyDictionary<CharacterClassType, Sprite> ClassIcons { get; }
    }
    
    [Serializable]
    public class UIConfigs : IUIConfigs
    {
        [TableList][SerializeField] List<ResourceIconData> resourceIcons;
        private Dictionary<ResourceType, Sprite> resourceIconsDictionary;

        [TableList][SerializeField] List<RarityUIData> rarities;
        private Dictionary<RarityType, RarityUIData> raritiesDictionary;
        
        [TableList][SerializeField] List<CharacterClassIconData> classIcons;
        private Dictionary<CharacterClassType, Sprite> classIconsDictionary;
        
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

        public IReadOnlyDictionary<RarityType, RarityUIData> Rarities
        {
            get
            {
                if (raritiesDictionary == null)
                {
                    raritiesDictionary = new Dictionary<RarityType, RarityUIData>();
                    foreach (var rarity in rarities)
                        raritiesDictionary.Add(rarity.Type, rarity);
                }
                return raritiesDictionary;
            }
        }
        
        public IReadOnlyDictionary<CharacterClassType, Sprite> ClassIcons
        {
            get
            {
                if (classIconsDictionary == null)
                {
                    classIconsDictionary = new Dictionary<CharacterClassType, Sprite>();
                    foreach (var @class in classIcons)
                        classIconsDictionary.Add(@class.Type, @class.Icon);
                }
                return classIconsDictionary;
            }
        }
    }
}