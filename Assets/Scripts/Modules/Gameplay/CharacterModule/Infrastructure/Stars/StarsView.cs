using UnityEngine;

namespace vikwhite
{
    public class StarsView : View<StarsHierarchy, StarsViewModel>
    {
        public StarsView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(StarsViewModel viewModel)
        {
            Bind(viewModel.Leaves, leaves => UpdateStars(leaves, star => star.Leaves));

            if (viewModel.SelectedLeaves != null)
                Bind(viewModel.SelectedLeaves, leaves => UpdateStars(leaves, star => star.SelectedLeaves));
        }

        private void UpdateStars(int leaves, System.Func<StarHierarchy, RectTransform[]> getLeaves)
        {
            var leavesLeft = leaves;
            if (_view.Stars == null) return;

            foreach (var star in _view.Stars)
                leavesLeft = SetStarLeaves(star, leavesLeft, getLeaves);
        }

        private static int SetStarLeaves(StarHierarchy star, int activeLeaves, System.Func<StarHierarchy, RectTransform[]> getLeaves)
        {
            var leavesLeft = Mathf.Max(0, activeLeaves);
            if (star == null) return leavesLeft;

            var leaves = getLeaves(star);
            if (leaves == null) return leavesLeft;

            foreach (var leaf in leaves)
            {
                if (leaf == null) continue;

                leaf.gameObject.SetActive(leavesLeft > 0);
                leavesLeft--;
            }

            return Mathf.Max(0, leavesLeft);
        }
    }
}
