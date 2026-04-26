using Unity.Mathematics;

namespace vikwhite.Utils
{
    public static class HexCoordinatesHandler
    {
        private const float HexWidth = 1f;
        private const float HalfHexWidth = HexWidth * 0.5f;
        private const float RowSpacing = 0.8660254f;

        public static float3 AxialToWorld(int q, int r)
        {
            return new float3(
                q * HexWidth + r * HalfHexWidth,
                0f,
                r * RowSpacing);
        }

        public static float3 AxialToWorld(int2 coordinates)
        {
            return AxialToWorld(coordinates.x, coordinates.y);
        }
    }
}