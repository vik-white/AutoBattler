using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BattleSystemGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = false;
        }
    }

    [UpdateInGroup(typeof(BattleSystemGroup))]
    public partial class CleanupSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(CleanupSystemGroup))]
    public partial class InitializeSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(InitializeSystemGroup))]
    public partial class SetupSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(SetupSystemGroup))]
    public partial class MovementSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(MovementSystemGroup))]
    public partial class CollisionSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(CollisionSystemGroup))]
    public partial class GameplaySystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(GameplaySystemGroup))]
    public partial class DeadSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(DeadSystemGroup))]
    public partial class StatusesSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(StatusesSystemGroup))]
    public partial class EffectsSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(EffectsSystemGroup))]
    public partial class CreateSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(CreateSystemGroup))]
    public partial class AnimationSystemGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(BattleSystemGroup))]
    [UpdateAfter(typeof(AnimationSystemGroup))]
    public partial class EventSystemGroup : ComponentSystemGroup { }
}
