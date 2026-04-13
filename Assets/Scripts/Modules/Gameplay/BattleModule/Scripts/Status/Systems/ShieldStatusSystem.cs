using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(StatusesSystemGroup))]
    [UpdateAfter(typeof(CreateStatusSystem))]
    public partial struct ShieldStatusSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (status, target, provider) in SystemAPI.Query<RefRW<Status>, RefRO<Target>, RefRO<Provider>>().WithAny<ShieldStatus>())
            {
                if(status.ValueRO.TimeSinceLastTick >= 0)
                    status.ValueRW.TimeSinceLastTick -= dt;
                else
                {
                    status.ValueRW.TimeSinceLastTick = status.ValueRO.Period;
                    ecb.CreateFrameEntity(new CreateEffect
                    {
                        Provider = provider.ValueRO.Value,
                        Target = target.ValueRO.Value, 
                        Data = new EffectData
                        {
                            Type = EffectType.Shield, 
                            Value  = status.ValueRO.Value
                        }
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}