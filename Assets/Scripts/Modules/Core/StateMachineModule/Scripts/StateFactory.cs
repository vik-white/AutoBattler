namespace vikwhite
{
    public interface IStateFactory<T> where T : IState
    {
        TS Get<TS>() where TS : IState;
    }
    
    public class StateFactory<T> : IStateFactory<T> where T : IState
    {
        private readonly DiContainer _container;

        public StateFactory(DiContainer container) {
            _container = container;
        }

        public TS Get<TS>() where TS : IState
        {
            return _container.Resolve<TS>();
        }
    }
}