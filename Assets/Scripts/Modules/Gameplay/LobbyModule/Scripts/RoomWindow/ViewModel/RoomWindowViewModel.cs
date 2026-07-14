namespace vikwhite
{
    public class RoomWindowViewModel : WindowViewModel<Room>
    {
        public string Title => $"{Model.Type} {Model.Level}";

        public RoomWindowViewModel(Room room) : base(room) { }
    }
}
