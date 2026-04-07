using UnityEngine;

namespace vikwhite
{
    public interface ILobbyState : IState { }
    
    public interface ILobbyStartState : ILobbyState { }

    public class LobbyStartState : ILobbyStartState, IUpdatable
    {
        public void Enter() 
        {
            Debug.Log("Entered Lobby");
        }

        public void Exit() { }

        public void Update() { }
    }
}