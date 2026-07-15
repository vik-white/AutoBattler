using UnityEngine;

namespace vikwhite
{
    public interface IRoomLineViewFactory
    {
        RoomLineView Get(RoomLineModel model, Transform parent);
    }

    public class RoomLineViewFactory : PooledViewFactory<RoomLineView, RoomLineViewModel>, IRoomLineViewFactory
    {
        private readonly IViewModelFactory _viewModelFactory;

        public RoomLineViewFactory(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public override string AssetName => "UI/Prefabs/RoomWindow/RoomLine";

        public RoomLineView Get(RoomLineModel model, Transform parent)
        {
            var viewModel = _viewModelFactory.CreateViewModel<RoomLineViewModel, RoomLineModel>(model);
            return Get(viewModel, parent);
        }
    }
}
