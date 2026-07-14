using UnityEngine;

namespace vikwhite
{
    public class RoomWindowView : WindowView<RoomWindowHierarchy, RoomWindowViewModel>
    {
        public RoomWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(RoomWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.CloseFadeButton, viewModel.Close);
            BindClick(_view.UpgradeButton, viewModel.OnUpgrade);
            Bind(viewModel.Level, level => _view.Title.text = $"{viewModel.Title} Lv.{level}");
        }
    }
}
