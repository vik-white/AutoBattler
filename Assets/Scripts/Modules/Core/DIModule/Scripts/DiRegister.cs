namespace vikwhite
{
    public class DiRegister
    {
        protected IDiContainer _container;

        public void Initialize(IDiContainer container)
        {
            _container = container;
            Register();
        }
        
        protected virtual void Register() { }
    }
}