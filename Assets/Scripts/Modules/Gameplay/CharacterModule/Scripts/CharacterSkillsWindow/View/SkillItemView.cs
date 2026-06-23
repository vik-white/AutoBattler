using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class SkillItemView : View<SkillItemHierarchy, SkillItemViewModel>
    {
        public SkillItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SkillItemViewModel viewModel)
        {
            _view.Icon.sprite = viewModel.Icon;
            BindClick(_view.Button, viewModel.OnSelect);
            Bind(viewModel.IsVisible, SetActive);
            Bind(viewModel.IsSelected, value => _view.Selected.SetActive(value));
            Bind(viewModel.IsLocked, value => {
                _view.Lock.SetActive(value);
                _view.LevelContainer.SetActive(!value);
            });
            Bind(viewModel.Level, value =>_view.Level.text = $"Lv.{value}");
        }
    }
}
