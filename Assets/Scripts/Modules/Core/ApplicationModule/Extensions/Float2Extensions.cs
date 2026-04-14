using Unity.Mathematics;

namespace vikwhite
{
    public static class Float2Extensions
    {
        public static float3 xoy(this float2 v)
        {
            return new float3(v.x, 0f, v.y);
        }
    }
}