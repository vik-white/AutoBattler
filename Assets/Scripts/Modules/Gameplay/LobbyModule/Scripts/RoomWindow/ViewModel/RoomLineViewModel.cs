namespace vikwhite
{
    public class RoomLineModel
    {
        public string Label { get; }
        public string Value { get; }

        public RoomLineModel(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }

    public class RoomLineViewModel : WindowViewModel<RoomLineModel>
    {
        public string Label => Model.Label;
        public string Value => Model.Value;

        public RoomLineViewModel(RoomLineModel model) : base(model) { }
    }
}
