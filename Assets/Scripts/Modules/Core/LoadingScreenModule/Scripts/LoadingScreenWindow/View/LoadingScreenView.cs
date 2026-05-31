using UnityEngine;

namespace vikwhite
{
    public class LoadingScreenView : WindowView<LoadingScreenHierarchy, LoadingScreenViewModel>
    {
        public LoadingScreenView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(LoadingScreenViewModel viewModel)
        {
            viewModel.OnProgressChanged += UpdateProgress;
            UpdateProgress(viewModel.Progress);
        }

        protected override void ReleaseViewModel()
        {
            if (ViewModel != null) ViewModel.OnProgressChanged -= UpdateProgress;
            base.ReleaseViewModel();
        }

        private void UpdateProgress(float progress)
        {
            _view.ProgressBar.SetProgress(progress);
        }
    }
}
