using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(StatusesSystemGroup))]
    [UpdateAfter(typeof(ApplyStatusesOnTargetsSystem))]
    public partial struct CreateStatusSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateStatus>>()) {
                var type = request.ValueRO.Data.Type;
                var status = ecb.CreateSceneEntity();
                ecb.AddComponent(status, new Status
                {
                    Value = request.ValueRO.Data.Value,
                    Duration = request.ValueRO.Data.Duration,
                    TileLeft = request.ValueRO.Data.Duration,
                    Period = request.ValueRO.Data.Period,
                });
                ecb.AddComponent(status, new Target{ Value = request.ValueRO.Target });
                ecb.AddComponent<Applied>(status);
                if (type == StatusType.Damage) ecb.AddComponent<StatusDamage>(status);
                if (type == StatusType.Heal) ecb.AddComponent<StatusHeal>(status);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}