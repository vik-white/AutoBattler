using UnityEngine;

namespace vikwhite
{
    public class MetaWindowView : WindowView<MetaWindowHierarchy, MetaWindowViewModel>
    {
        private readonly IMetaItemViewFactory _itemViewFactory;

        public MetaWindowView(GameObject view, IMetaItemViewFactory itemViewFactory) : base(view)
        {
            _itemViewFactory = itemViewFactory;
        }

        protected override void UpdateViewModel(MetaWindowViewModel viewModel)
        {
            _view.ItemContainer.ClearChildren();
            foreach (var character in viewModel.Characters)
                _itemViewFactory.Get(character, _view.ItemContainer);
        }
    }
}
