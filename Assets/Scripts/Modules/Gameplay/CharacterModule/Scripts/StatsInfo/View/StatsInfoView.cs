using UnityEngine;

namespace vikwhite
{
    public class StatsInfoView : View<StatsInfoHierarchy, StatsInfoViewModel>
    {
        public StatsInfoView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(StatsInfoViewModel viewModel)
        {
            if (_view.Stats == null) return;

            var count = Mathf.Min(_view.Stats.Length, viewModel.Stats.Count);
            for (int i = 0; i < count; i++)
                CreateView<StatView, StatHierarchy>(_view.Stats[i]).Initialize(viewModel.Stats[i]);
        }
    }
}
