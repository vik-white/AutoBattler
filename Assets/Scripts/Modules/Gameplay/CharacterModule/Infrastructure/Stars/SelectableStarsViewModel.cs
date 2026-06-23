using UniRx;

namespace vikwhite
{
    public class SelectableStarsModel
    {
        public IReadOnlyReactiveProperty<int> Leaves { get; }
        public IReadOnlyReactiveProperty<int> SelectedLeaves { get; }

        public SelectableStarsModel(IReadOnlyReactiveProperty<int> leaves, IReadOnlyReactiveProperty<int> selectedLeaves)
        {
            Leaves = leaves;
            SelectedLeaves = selectedLeaves;
        }
    }

    public class SelectableStarsViewModel : ViewModel<SelectableStarsModel>
    {
        public IReadOnlyReactiveProperty<int> Leaves => Model.Leaves;
        public IReadOnlyReactiveProperty<int> SelectedLeaves => Model.SelectedLeaves;

        public SelectableStarsViewModel(SelectableStarsModel model) : base(model)
        {
        }
    }
}
