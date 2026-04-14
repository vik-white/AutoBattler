using Unity.Mathematics;
using UnityEngine;

namespace vikwhite.Utils
{
    public static class MathHandler
    {
        public static float2 GetRandomPointInRadius(float2 center, float radius)
        {
            float angle = UnityEngine.Random.value * Mathf.PI * 2f;
            float distance = Mathf.Sqrt(UnityEngine.Random.value) * radius;

            float x = Mathf.Cos(angle) * distance;
            float y = Mathf.Sin(angle) * distance;

            return center + new float2(x, y);
        }
    }
}