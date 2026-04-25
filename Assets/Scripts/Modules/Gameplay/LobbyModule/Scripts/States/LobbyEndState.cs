namespace vikwhite
{
    public interface ILobbyEndState : ILobbyState { }
    
    public class LobbyEndState : ILobbyEndState
    {
        public void Enter() { }

        public void Exit() { }
    }
}