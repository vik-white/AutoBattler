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
            EnsureReferences();
            if (_view.Icon != null && viewModel.Icon != null)
                _view.Icon.sprite = viewModel.Icon;

            if (_view.Button != null) BindClick(_view.Button, viewModel.OnSelect);
            Bind(viewModel.IsVisible, SetActive);
            Bind(viewModel.IsSelected, value =>
            {
                if (_view.Selected != null) _view.Selected.SetActive(value);
            });
            Bind(viewModel.IsLocked, value =>
            {
                if (_view.Lock != null) _view.Lock.SetActive(value);
            });
            Bind(viewModel.Level, value =>
            {
                if (_view.Level != null) _view.Level.text = $"Lv.{value}";
            });
        }

        private void EnsureReferences()
        {
            if (_view.Icon == null) _view.Icon = FindInChildren<Image>("SkillIcon");
            if (_view.Level == null) _view.Level = FindText("SkillLevel");
            if (_view.Lock == null) _view.Lock = FindDirectChild("Lock")?.gameObject;
            if (_view.Selected == null) _view.Selected = FindDirectChild("SkillSelected")?.gameObject;

            if (_view.Button == null) _view.Button = GameObject.GetComponent<Button>();
            if (_view.Button == null) _view.Button = GameObject.AddComponent<Button>();
        }

        private Transform FindDirectChild(string childName)
        {
            var root = GameObject.transform;
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name == childName) return child;
            }

            return null;
        }

        private T FindInChildren<T>(string childName) where T : Component
        {
            var components = GameObject.GetComponentsInChildren<T>(true);
            foreach (var component in components)
            {
                if (component.name == childName) return component;
            }

            return null;
        }

        private TMP_Text FindText(string childName)
        {
            var texts = GameObject.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in texts)
            {
                if (text.name == childName) return text;
            }

            return null;
        }
    }
}
