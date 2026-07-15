using UnityEngine;

namespace vikwhite
{
    public interface IRoomProductionViewFactory
    {
        RoomProductionView Get(RoomProductionModel model, Transform parent);
    }

    public class RoomProductionViewFactory : PooledViewFactory<RoomProductionView, RoomProductionViewModel>,
        IRoomProductionViewFactory
    {
        private readonly IViewModelFactory _viewModelFactory;

        public RoomProductionViewFactory(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public override string AssetName => "UI/Prefabs/RoomWindow/RoomProduction";

        public RoomProductionView Get(RoomProductionModel model, Transform parent)
        {
            var viewModel = _viewModelFactory
                .CreateViewModel<RoomProductionViewModel, RoomProductionModel>(model);
            return Get(viewModel, parent);
        }
    }
}
