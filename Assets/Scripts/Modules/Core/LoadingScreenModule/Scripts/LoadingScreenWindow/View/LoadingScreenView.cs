using UnityEngine;

namespace vikwhite
{
    public class LoadingScreenView : WindowView<LoadingScreenHierarchy, LoadingScreenViewModel>
    {
        public LoadingScreenView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(LoadingScreenViewModel viewModel)
        {
            Bind(viewModel.Progress, UpdateProgress);
        }

        private void UpdateProgress(float progress)
        {
            _view.ProgressBar.SetProgress(progress);
        }
    }
}
