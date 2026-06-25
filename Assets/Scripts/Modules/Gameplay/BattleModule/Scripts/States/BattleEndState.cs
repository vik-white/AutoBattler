using vikwhite.ECS;

namespace vikwhite
{
    public interface IBattleEndState : IBattleState { }
    
    public class BattleEndState : IBattleEndState
    {
        public void Enter() 
        {
            TimeSystem.Reset();
            ECSWorld.SetManagedEnabled<BattleSystemGroup>(false);
            ECSWorld.DestroyScene();
        }

        public void Exit() { }
    }
}
