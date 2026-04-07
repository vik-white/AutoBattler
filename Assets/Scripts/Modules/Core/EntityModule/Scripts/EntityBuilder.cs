namespace vikwhite
{
    public class EntityBuilder
    {
        private readonly DiContainer _container = DI.Resolve<DiContainer>();
        private readonly Entity _entity;
        private Component _component;

        public EntityBuilder()
        {
            _entity = _container.Resolve<Entity>();
        }
        
        public EntityBuilder Add<T>() where T : Component
        {
            _component = _container.Resolve<T>();
            _entity.Add((T)_component);
            return this;
        }
        
        public EntityBuilder Add<T>(out T component) where T : Component
        {
            component = _container.Resolve<T>();
            _component = component;
            _entity.Add((T)_component);
            return this;
        }
        
        public EntityBuilder Add<T, TArg>(TArg arg) where T : Component
        {
            _component = _container.Resolve<T, TArg>(arg);
            _entity.Add((T)_component);
            return this;
        }
        
        public EntityBuilder Add<T, TArg>(TArg arg, out T component) where T : Component
        {
            component = _container.Resolve<T, TArg>(arg);
            _component = component;
            _entity.Add((T)_component);
            return this;
        }
        
        public Entity Build() => _entity;
    }
}