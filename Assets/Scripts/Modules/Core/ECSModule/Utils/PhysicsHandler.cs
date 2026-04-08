using Unity.Mathematics;
using Unity.Physics;

namespace vikwhite
{
    public static class PhysicsHandler
    {
        public static PhysicsMass CreateFreezeRotation(MassProperties massProperties, float mass = 1f)
        {
            var physicsMass = PhysicsMass.CreateDynamic(massProperties, mass);
            physicsMass.InverseInertia = float3.zero;
            return physicsMass;
        }
    }
}