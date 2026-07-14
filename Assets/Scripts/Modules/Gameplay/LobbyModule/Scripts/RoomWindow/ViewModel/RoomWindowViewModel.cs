namespace vikwhite
{
    public class RoomWindowViewModel : WindowViewModel<Room>
    {
        public string Title => $"{Model.Type} Lv.{Model.Level.Value}";

        public RoomWindowViewModel(Room room) : base(room) { }
    }
}
