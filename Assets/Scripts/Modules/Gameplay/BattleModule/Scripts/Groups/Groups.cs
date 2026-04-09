using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class CleanupSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(CleanupSystemGroup))]
    public partial class InitializeSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(InitializeSystemGroup))]
    public partial class SetupSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(SetupSystemGroup))]
    public partial class MovementSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(MovementSystemGroup))]
    public partial class CollisionSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(CollisionSystemGroup))]
    public partial class GameplaySystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(GameplaySystemGroup))]
    public partial class DeadSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(DeadSystemGroup))]
    public partial class EffectsSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(EffectsSystemGroup))]
    public partial class CreateSystemGroup : ComponentSystemGroup { }
    
    [UpdateAfter(typeof(CreateSystemGroup))]
    public partial class EventSystemGroup : ComponentSystemGroup { }
}