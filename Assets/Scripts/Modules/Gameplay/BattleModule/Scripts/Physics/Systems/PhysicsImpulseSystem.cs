using Unity.Entities;
using Unity.Mathematics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup), OrderLast = true)]
    public partial struct PhysicsImpulseUpSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            foreach (var (impulse, velocity) in SystemAPI.Query<RefRO<Impulse>, RefRW<ExternalVelocity>>()) 
            {
                velocity.ValueRW.Value += impulse.ValueRO.Value;
            } 
        }
    }
}