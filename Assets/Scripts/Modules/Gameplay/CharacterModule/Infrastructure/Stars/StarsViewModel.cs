using UniRx;

namespace vikwhite
{
    public class StarsViewModel : ViewModel<IReadOnlyReactiveProperty<int>>
    {
        public IReadOnlyReactiveProperty<int> Leaves => Model;

        public StarsViewModel(IReadOnlyReactiveProperty<int> leaves) : base(leaves) { }
    }
}
