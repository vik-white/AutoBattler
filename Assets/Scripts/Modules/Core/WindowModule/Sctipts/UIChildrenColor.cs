using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace vikwhite
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/UI Children Color")]
    public class UIChildrenColor : MonoBehaviour
    {
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private bool _includeInactive = true;
        [SerializeField] private bool _includeSelf = true;

        public Color Color
        {
            get => _color;
            set
            {
                if (_color == value) return;

                _color = value;
                ApplyColor();
            }
        }

        private void OnEnable()
        {
            ApplyColor();
        }

        private void OnValidate()
        {
            ApplyColor();
        }

        private void OnTransformChildrenChanged()
        {
            ApplyColor();
        }

        [ContextMenu("Apply Color")]
        public void ApplyColor()
        {
            ApplyToImages();
            ApplyToTexts();
        }

        private void ApplyToImages()
        {
            var images = GetComponentsInChildren<Image>(_includeInactive);

            foreach (var image in images)
            {
                if (!_includeSelf && image.gameObject == gameObject) continue;

                SetColor(image, _color);
            }
        }

        private void ApplyToTexts()
        {
            var texts = GetComponentsInChildren<TMP_Text>(_includeInactive);

            foreach (var text in texts)
            {
                if (!_includeSelf && text.gameObject == gameObject) continue;

                SetColor(text, _color);
            }
        }

        private static void SetColor(Image image, Color color)
        {
            if (image.color == color) return;

            image.color = color;
            MarkDirty(image);
        }

        private static void SetColor(TMP_Text text, Color color)
        {
            if (text.color == color) return;

            text.color = color;
            MarkDirty(text);
        }

        private static void MarkDirty(Object target)
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;

            UnityEditor.EditorUtility.SetDirty(target);

            if (target is Component component && component.gameObject.scene.IsValid())
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(component.gameObject.scene);

            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(target))
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(target);
#endif
        }
    }
}
