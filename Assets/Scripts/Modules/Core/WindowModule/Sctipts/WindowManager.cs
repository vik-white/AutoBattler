using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace vikwhite
{
    public interface IWindowManager
    {
        void ShowWindow(IWindowView view);
        void CloseWindow(IWindowView view);
        void CloseAllWindows();
    }
    
    public class WindowManager : IWindowManager
    {
        private readonly HashSet<IWindowView> _openedWindows = new();

        public void ShowWindow(IWindowView view)
        {
            _openedWindows.Add(view);
            view.ShowInternal();
        }

        public void CloseWindow(IWindowView view)
        {
            view.CloseInternal();
            _openedWindows.Remove(view);
        }

        public void CloseAllWindows()
        {
            foreach (var view in _openedWindows.ToList())
            {
                if (view.ViewModel != null)
                    view.ViewModel.Close();
                else
                    CloseWindow(view);
            }
        }
    }
}
