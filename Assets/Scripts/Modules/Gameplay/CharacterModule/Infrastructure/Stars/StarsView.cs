using UnityEngine;

namespace vikwhite
{
    public class StarsView : View<StarsHierarchy, StarsViewModel>
    {
        public StarsView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(StarsViewModel viewModel)
        {
            Bind(viewModel.Leaves, UpdateStars);
        }

        private void UpdateStars(int leaves)
        {
            var leavesLeft = leaves;
            if (_view.Stars == null) return;

            foreach (var star in _view.Stars)
                leavesLeft = SetStarLeaves(star, leavesLeft);
        }

        private static int SetStarLeaves(StarHierarchy star, int activeLeaves)
        {
            var leavesLeft = Mathf.Max(0, activeLeaves);
            if (star == null || star.Leaves == null) return leavesLeft;

            foreach (var leaf in star.Leaves)
            {
                if (leaf == null) continue;

                leaf.gameObject.SetActive(leavesLeft > 0);
                leavesLeft--;
            }

            return Mathf.Max(0, leavesLeft);
        }
    }
}
