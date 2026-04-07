namespace vikwhite
{
    public interface IWindowManager
    {
        void ShowWindow(IWindowView view);
        void CloseWindow(IWindowView view);
    }
    
    public class WindowManager : IWindowManager
    {
        public void ShowWindow(IWindowView view)
        {
            view.ShowInternal();
        }

        public void CloseWindow(IWindowView view)
        {
            view.CloseInternal();
        }
    }
}