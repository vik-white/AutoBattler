using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(StatusesSystemGroup))]
    [UpdateAfter(typeof(CreateStatusSystem))]
    public partial struct StatusSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<Time>()) return;

            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (status, target, provider) in SystemAPI.Query<RefRW<Status>, RefRO<Target>, RefRO<Provider>>())
            {
                if(status.ValueRO.TimeSinceLastTick >= 0)
                    status.ValueRW.TimeSinceLastTick -= dt;
                else
                {
                    status.ValueRW.TimeSinceLastTick = status.ValueRO.Period;
                    ecb.CreateFrameEntity(new CreateEffect
                    {
                        Ability = status.ValueRO.Ability,
                        Provider = provider.ValueRO.Value,
                        Target = target.ValueRO.Value, 
                        Data = new EffectData
                        {
                            Type = status.ValueRO.Type, 
                            Value  = status.ValueRO.Value
                        }
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
