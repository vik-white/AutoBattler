using UniRx;

namespace vikwhite
{
    public class StarsModel
    {
        public IReadOnlyReactiveProperty<int> Leaves { get; }
        public IReadOnlyReactiveProperty<int> SelectedLeaves { get; }

        public StarsModel(IReadOnlyReactiveProperty<int> leaves, IReadOnlyReactiveProperty<int> selectedLeaves = null)
        {
            Leaves = leaves;
            SelectedLeaves = selectedLeaves;
        }
    }

    public class StarsViewModel : ViewModel<StarsModel>
    {
        public IReadOnlyReactiveProperty<int> Leaves => Model.Leaves;
        public IReadOnlyReactiveProperty<int> SelectedLeaves => Model.SelectedLeaves;

        public StarsViewModel(StarsModel model) : base(model) { }
    }
}
