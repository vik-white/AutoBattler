using System;

namespace vikwhite
{
    public interface IStateMachine<T> where T : IState
    {
        void SwitchState<TS>() where TS : IState;
    }
    
    public class StateMachine<T> : IStateMachine<T> where T : IState
    {
        private readonly IStateFactory<T> _gameStateFactory;
        private IState _currentState;

        public StateMachine(IStateFactory<T> gameStateFactory) {
            _gameStateFactory = gameStateFactory;
        }

        public void SwitchState<TS>() where TS : IState {
            _currentState?.Exit();
            _currentState = _gameStateFactory.Get<TS>();
            _currentState.Enter();
        }
    }
}