using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    public class CharacterSkillsWindowView : WindowView<CharacterSkillsWindowHierarchy, CharacterSkillsWindowViewModel>
    {
        public CharacterSkillsWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(CharacterSkillsWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            EnsureReferences();
            if (_view.StatsButton != null) BindClick(_view.StatsButton, viewModel.OnOpenStats);
            if (_view.UpgradeButton != null) BindClick(_view.UpgradeButton, viewModel.OnUpgradeSkill);

            Bind(viewModel.SkillName, value =>
            {
                if (_view.SkillName != null) _view.SkillName.text = value;
            });
            Bind(viewModel.SkillDescription, value =>
            {
                if (_view.SkillDescription != null) _view.SkillDescription.text = value;
            });
            Bind(viewModel.SkillUpgradePrice, value =>
            {
                if (_view.SkillUpgradePrice != null) _view.SkillUpgradePrice.text = value;
            });
            Bind(viewModel.BooksAmount, value =>
            {
                if (_view.BooksAmount != null) _view.BooksAmount.text = value.ToString();
            });
            Bind(viewModel.CanUpgradeSkill, value =>
            {
                if (_view.UpgradeButton != null) _view.UpgradeButton.interactable = value;
            });

            if (_view.Name != null) _view.Name.text = viewModel.Name;
            if (_view.Image != null) _view.Image.sprite = viewModel.Image;
            if (_view.ClassIcon != null) _view.ClassIcon.sprite = viewModel.ClassIcon;
            CreateSkillItemViews(viewModel);
        }

        private void EnsureReferences()
        {
            if (_view.Name == null) _view.Name = FindText("Name");
            if (_view.SkillName == null) _view.SkillName = FindText("SkillName");
            if (_view.SkillDescription == null) _view.SkillDescription = FindText("SkillDescription");
            if (_view.Image == null) _view.Image = FindImage("Hero");
            if (_view.ClassIcon == null) _view.ClassIcon = FindImage("ClassIcon");
            if (_view.StatsButton == null) _view.StatsButton = FindTabButton("Stats");
            if (_view.UpgradeButton == null) _view.UpgradeButton = FindNamedButton("ButtonBlue2Line");
            if (_view.SkillUpgradePrice == null) _view.SkillUpgradePrice = FindUpgradePriceText(_view.UpgradeButton);

            var redeemButton = FindTransform("RedeemButton");
            if (_view.BooksAmount == null) _view.BooksAmount = FindText("Amount", redeemButton);

            if (_view.SkillItems == null || _view.SkillItems.Length == 0)
                _view.SkillItems = FindSkillItems();
        }

        private void CreateSkillItemViews(CharacterSkillsWindowViewModel viewModel)
        {
            if (_view.SkillItems == null) return;
            var count = Mathf.Min(_view.SkillItems.Length, viewModel.Skills.Count);

            for (var i = 0; i < count; i++)
                CreateView<SkillItemView, SkillItemHierarchy>(_view.SkillItems[i]).Initialize(viewModel.Skills[i]);
        }

        private SkillItemHierarchy[] FindSkillItems()
        {
            var items = new List<SkillItemHierarchy>();
            var rects = GameObject.GetComponentsInChildren<RectTransform>(true);

            foreach (var rect in rects)
            {
                if (rect.parent != _view.transform || !rect.name.StartsWith("SkillItem")) continue;

                var hierarchy = rect.GetComponent<SkillItemHierarchy>();
                if (hierarchy == null) hierarchy = rect.gameObject.AddComponent<SkillItemHierarchy>();
                items.Add(hierarchy);
            }

            items.Sort((first, second) =>
            {
                var firstRect = (RectTransform)first.transform;
                var secondRect = (RectTransform)second.transform;
                var firstColumn = firstRect.anchoredPosition.x >= 0 ? 1 : 0;
                var secondColumn = secondRect.anchoredPosition.x >= 0 ? 1 : 0;
                if (firstColumn != secondColumn) return firstColumn.CompareTo(secondColumn);
                return secondRect.anchoredPosition.y.CompareTo(firstRect.anchoredPosition.y);
            });

            return items.ToArray();
        }

        private Button FindTabButton(string label)
        {
            var texts = GameObject.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in texts)
            {
                if (text.text != label) continue;

                var tab = FindParent(text.transform, "Tab");
                if (tab == null) continue;

                var button = tab.GetComponent<Button>();
                if (button == null) button = tab.gameObject.AddComponent<Button>();
                if (button.targetGraphic == null) button.targetGraphic = tab.GetComponentInChildren<Graphic>(true);
                return button;
            }

            return null;
        }

        private Transform FindParent(Transform start, string parentName)
        {
            var current = start.parent;
            while (current != null && current != _view.transform)
            {
                if (current.name == parentName) return current;
                current = current.parent;
            }

            return null;
        }

        private Button FindNamedButton(string objectName)
        {
            var target = FindTransform(objectName);
            if (target == null) return null;

            var button = target.GetComponent<Button>();
            if (button == null) button = target.GetComponentInChildren<Button>(true);
            if (button == null) button = target.gameObject.AddComponent<Button>();
            if (button.targetGraphic == null) button.targetGraphic = button.GetComponentInChildren<Graphic>(true);
            return button;
        }

        private TMP_Text FindUpgradePriceText(Button button)
        {
            if (button == null) return null;

            var texts = button.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in texts)
            {
                if (text.text.Contains("/")) return text;
            }

            return texts.Length > 0 ? texts[0] : null;
        }

        private Transform FindTransform(string objectName)
        {
            var transforms = GameObject.GetComponentsInChildren<Transform>(true);
            foreach (var item in transforms)
            {
                if (item.name == objectName) return item;
            }

            return null;
        }

        private TMP_Text FindText(string objectName, Transform root = null)
        {
            var targetRoot = root == null ? GameObject.transform : root;
            var texts = targetRoot.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in texts)
            {
                if (text.name == objectName) return text;
            }

            return null;
        }

        private Image FindImage(string objectName)
        {
            var images = GameObject.GetComponentsInChildren<Image>(true);
            foreach (var image in images)
            {
                if (image.name == objectName) return image;
            }

            return null;
        }
    }
}
