using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace vikwhite
{
    public interface IView : IDisposable
    {
        GameObject GameObject { get; }
        void DisposeAndDestroy();
    }
    
    public interface IView<in TViewModel> : IView
    {
        void Initialize(TViewModel viewModel);
    }
    
    public class View : DisposableCollector, IView
    {
        private readonly IViewFactory _viewFactory = DI.Resolve<IViewFactory>();
        private readonly List<View> _childViews = new();
        public GameObject GameObject { get; }
        
        protected View(GameObject gameObject) => GameObject = gameObject;

        public virtual void SetActive(bool active) => GameObject.SetActive(active);

        protected TView CreateView<TView, THierarchy>(THierarchy hierarchy) where TView : View<THierarchy> where THierarchy : MonoBehaviour
        {
            TView view = _viewFactory.CreateView<TView, THierarchy>(hierarchy);
            _childViews.Add(view);
            AddDisposable(view);
            return view;
        }

        public virtual void Release()
        {
            for (var i = 0; i < _childViews.Count; i++) _childViews[i].Release();
        }
        
        public void DisposeAndDestroy()
        {
            Dispose();
            GameObject.Destroy(GameObject);
        }

        public override void Dispose()
        {
            Release();
            base.Dispose();
        }
    }
    
    public abstract class View<TView> : View where TView : MonoBehaviour
    {
        protected readonly TView _view;

        protected View(GameObject view) : base(view)
        {
            _view = view.GetComponent<TView>();
            if(_view == null) throw new Exception($"View {view.name} does not have component {typeof(TView).Name}");
        }
        
        public TView GetView()
        {
            return _view;
        }
    }
    
    public abstract class View<TView, TViewModel> : View<TView>, IView<TViewModel> where TView : MonoBehaviour where TViewModel : class, IViewModel
    {
        private readonly List<IDisposable> _bindDisposables = new();
        private readonly List<(Button button, UnityAction onClick)> _bindButtonDisposables = new();
        public TViewModel BaseViewModel { get; private set; }
        
        protected View(GameObject view) : base(view) { }

        public void Initialize(TViewModel viewModel)
        {
            BaseViewModel = viewModel;
            UpdateViewModel(viewModel);
        }
        
        protected abstract void UpdateViewModel(TViewModel viewModel);

        protected void Register(IDisposable disposable)
        {
            _bindDisposables.Add(disposable);
        }
        
        protected void BindClick(Button button, UnityAction onClick)
        {
            button.onClick.AddListener(onClick);
            _bindButtonDisposables.Add((button, onClick));
        }

        protected void Bind<T>(IReadOnlyReactiveProperty<T> field, Action<T> onChange)
        {
            _bindDisposables.Add(field.Subscribe(onChange));
        }
        
        protected virtual void ReleaseViewModel()
        {
            for (int i = 0; i < _bindDisposables.Count; i++)
            {
                _bindDisposables[i].Dispose();
            }
            _bindDisposables.Clear();
            
            for (var index = 0; index < _bindButtonDisposables.Count; index++)
            {
                (Button button, UnityAction onClick) = _bindButtonDisposables[index];
                button.onClick.RemoveListener(onClick);
            }

            _bindButtonDisposables.Clear();
        }
        
        public sealed override void Release()
        {
            base.Release();
            if (BaseViewModel == null) return;
            ReleaseViewModel();
            BaseViewModel.Dispose();
            BaseViewModel = null;
        }
    }
}