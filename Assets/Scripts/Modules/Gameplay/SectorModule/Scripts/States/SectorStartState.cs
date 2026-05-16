namespace vikwhite
{
    public interface ISectorState : IState { }

    public interface ISectorStartState : ISectorState { }

    public class SectorStartState : ISectorStartState
    {
        private readonly ISectorWindow _sectorWindow;

        public SectorStartState(ISectorWindow sectorWindow)
        {
            _sectorWindow = sectorWindow;
        }

        public void Enter()
        {
            _sectorWindow.ShowWindow();
        }

        public void Exit()
        {
            _sectorWindow.CloseWindow();
        }
    }
}
