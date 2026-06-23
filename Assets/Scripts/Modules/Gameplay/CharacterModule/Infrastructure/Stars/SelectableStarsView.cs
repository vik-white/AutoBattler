using System;
using UnityEngine;

namespace vikwhite
{
    public class SelectableStarsView : View<StarsHierarchy, SelectableStarsViewModel>
    {
        public SelectableStarsView(GameObject view) : base(view)
        {
        }

        protected override void UpdateViewModel(SelectableStarsViewModel viewModel)
        {
            Bind(viewModel.Leaves, leaves => UpdateStars(leaves, star => star.Leaves));
            Bind(viewModel.SelectedLeaves, leaves => UpdateStars(leaves, star => star.SelectedLeaves));
        }

        private void UpdateStars(int leaves, Func<StarHierarchy, RectTransform[]> getLeaves)
        {
            foreach (var star in _view.Stars)
            {
                var starLeaves = getLeaves(star);

                if (starLeaves == null)
                {
                    continue;
                }

                for (var i = 0; i < starLeaves.Length; i++)
                {
                    var leaf = starLeaves[i];

                    if (leaf == null)
                    {
                        continue;
                    }

                    leaf.gameObject.SetActive(leaves > 0);
                    leaves--;
                }
            }
        }
    }
}
