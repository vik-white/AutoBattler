using System;
using UnityEngine;

namespace vikwhite
{
    public interface IWindowView : IView
    {
        IWindowViewModel ViewModel { get; }
        bool IsShown { get; }
        bool IsClosed { get; }
        void ShowInternal();
        void HideInternal();
        void CloseInternal();
    }
    
    public interface IWindowView<in T> : IView<T>, IWindowView where T:IWindowViewModel { }
    
    public class WindowView<TView, TViewModel> : View<TView, TViewModel>, IWindowView<TViewModel>
        where TViewModel : class, IWindowViewModel
        where TView : MonoBehaviour
    {
        public IWindowViewModel ViewModel => BaseViewModel;
        public bool IsShown { get; set; }
        public bool IsClosed { get; set; }
        
        public WindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(TViewModel viewModel) { }
        
        protected void CloseWindow()
        {
            ViewModel.Close();
        }
        
        public void ShowInternal()
        {
            IsClosed = false;
            GameObject.transform.SetAsLastSibling();
            if (!IsShown)
            {
                IsShown = true;
                OnShown();
                SetActive(true);
            }
        }
        
        public void HideInternal()
        {
            if (!IsShown) return;
            IsShown = false;
            OnHide();
            SetActive(false);
        }
        
        public void CloseInternal()
        {
            IsClosed = true;
            if (IsShown)
            {
                IsShown = false;
                OnHide();
                SetActive(false);
            }
            Release();
        }
        
        protected virtual void OnShown() { }

        protected virtual void OnHide() { }
    }
}
