using System;
using System.Collections.Generic;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite.Data
{
    [Serializable]
    public class AbilityData
    {
        public float Cooldown;
        public float Radius;
        public GameObject Prefab;
        public List<StatData> Stats;
        public List<EffectData> Effects;
        public ProjectileData Projectile;
    }
}