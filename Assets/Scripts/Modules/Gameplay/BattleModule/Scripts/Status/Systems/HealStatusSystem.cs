using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(StatusesSystemGroup))]
    [UpdateAfter(typeof(CreateStatusSystem))]
    public partial struct HealStatusSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (status, target) in SystemAPI.Query<RefRW<Status>, RefRO<Target>>().WithAny<StatusHeal>())
            {
                if(status.ValueRO.TimeSinceLastTick >= 0)
                    status.ValueRW.TimeSinceLastTick -= dt;
                else
                {
                    status.ValueRW.TimeSinceLastTick = status.ValueRO.Period;
                    ecb.CreateFrameEntity(new CreateEffect
                    {
                        Target = target.ValueRO.Value, 
                        Data = new EffectData
                        {
                            Type = EffectType.Heal, 
                            Value  = status.ValueRO.Value
                        }
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}