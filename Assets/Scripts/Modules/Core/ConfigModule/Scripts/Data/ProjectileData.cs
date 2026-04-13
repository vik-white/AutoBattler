using System;
using System.Runtime.InteropServices;

namespace vikwhite.Data
{
    [Serializable]
    public struct ProjectileData
    {
        public int Count;
        public float Speed;
        public int Pierce;
        public float Scale;
        public float OrbitRadius;
        public float Lifetime;
    }
}