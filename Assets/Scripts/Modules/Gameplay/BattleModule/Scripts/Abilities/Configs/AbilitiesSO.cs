using System.Collections.Generic;
using UnityEngine;

namespace vikwhite.Data
{
    [CreateAssetMenu(fileName = "Abilities", menuName = "vikwhite/Abilities")]
    public class AbilitiesSO : ScriptableObject
    {
        private static AbilitiesSO _instance;
        public static AbilitiesSO Instance => _instance ??= Resources.Load<AbilitiesSO>("Configs/Abilities");
        
        public List<AbilitySO> Array;
    }
}