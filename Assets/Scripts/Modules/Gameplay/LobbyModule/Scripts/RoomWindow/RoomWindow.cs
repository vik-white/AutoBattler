namespace vikwhite
{
    public interface IRoomWindow : IWindowPresenter
    {
        void ShowWindow(Room room);
    }

    public class RoomWindow : WindowPresenter<RoomWindowView, RoomWindowViewModel>, IRoomWindow
    {
        public override string AssetName => "UI/Prefabs/RoomWindow/RoomWindow";

        public void ShowWindow(Room room)
        {
            var window = _viewModelFactory.CreateViewModel<RoomWindowViewModel, Room>(room);
            ShowWindow(window);
        }
    }
}
