namespace vikwhite
{
    public class RoomLineModel
    {
        public string Label { get; }
        public string Value { get; }
        public bool? RequirementMet { get; }

        public RoomLineModel(string label, string value, bool? requirementMet = null)
        {
            Label = label;
            Value = value;
            RequirementMet = requirementMet;
        }
    }

    public class RoomLineViewModel : WindowViewModel<RoomLineModel>
    {
        public string Label => Model.Label;
        public string Value => Model.Value;
        public bool? RequirementMet => Model.RequirementMet;

        public RoomLineViewModel(RoomLineModel model) : base(model) { }
    }
}
