namespace vikwhite
{
    public interface ISectorEndState : ISectorState { }

    public class SectorEndState : ISectorEndState
    {
        public void Enter() { }

        public void Exit() { }
    }
}
