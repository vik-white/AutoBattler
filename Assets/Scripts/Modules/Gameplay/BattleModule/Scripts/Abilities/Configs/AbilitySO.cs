using System;
using System.Collections.Generic;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite.Data
{
    [CreateAssetMenu(fileName = "Ability", menuName = "vikwhite/Ability")]
    public class AbilitySO : ScriptableObject
    {
        public AbilityID ID;
        public List<AbilityLevelData> Levels;
    }
}