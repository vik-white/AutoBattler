
using UniRx;

namespace vikwhite
{
    public class ResourceViewModel: WindowViewModel<Resource>
    {
        public IReadOnlyReactiveProperty<int> Amount => Model.Amount;
        public ResourceType Type;
        
        public ResourceViewModel(Resource model) : base(model)
        {
            Type = model.Type;
        }
    }
}