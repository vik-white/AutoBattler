using UnityEngine;

namespace vikwhite
{
    public interface IRoomProgressViewFactory
    {
        RoomProgressView Get(RoomProgressModel model, Transform parent);
    }

    public class RoomProgressViewFactory : PooledViewFactory<RoomProgressView, RoomProgressViewModel>,
        IRoomProgressViewFactory
    {
        private readonly IViewModelFactory _viewModelFactory;

        public RoomProgressViewFactory(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public override string AssetName => "UI/Prefabs/RoomWindow/RoomProgress";

        public RoomProgressView Get(RoomProgressModel model, Transform parent)
        {
            var viewModel = _viewModelFactory
                .CreateViewModel<RoomProgressViewModel, RoomProgressModel>(model);
            return Get(viewModel, parent);
        }
    }
}
