using UniRx;

namespace vikwhite
{
    public class MetaStarsViewModel : ViewModel<IReadOnlyReactiveProperty<int>>
    {
        public IReadOnlyReactiveProperty<int> Leaves => Model;

        public MetaStarsViewModel(IReadOnlyReactiveProperty<int> leaves) : base(leaves) { }
    }
}
