using System;

namespace vikwhite
{
    public interface IWindowViewModel : IViewModel
    {
        event Action OnClose;
        void Close();
    }

    public interface IWindowViewModel<T> : IWindowViewModel, IViewModel<T> { }

    public abstract class WindowViewModel : ViewModel, IWindowViewModel
    {
        public event Action OnClose;

        public virtual void Close()
        {
            OnClose?.Invoke();
        }
    }

    public abstract class WindowViewModel<T> : ViewModel<T>, IWindowViewModel<T>
    {
        public event Action OnClose;
        
        protected WindowViewModel(T model) : base(model) { }
        
        public virtual void Close()
        {
            OnClose?.Invoke();
        }
    }
}
