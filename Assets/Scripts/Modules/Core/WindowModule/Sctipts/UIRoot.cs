using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    public enum UILayer { WORLD, GUI, WINDOW, FLYTEXT, DRAG, POPUP }
    
    public interface IUIRoot
    {
        Vector2 CanvasSize { get; }
        Vector2 CanvasCenter { get; }

        public void Initialize(RectTransform rectTransform);
        RectTransform GetLayer(UILayer layer);
    }
    
    public class UIRoot : IUIRoot
    {
        private RectTransform _rectTransform;
        private Dictionary<UILayer, RectTransform> _layers;
        public Vector2 CanvasSize => _rectTransform.sizeDelta;
        public Vector2 CanvasCenter => CanvasSize * 0.5f;
        
        public void Initialize(RectTransform rectTransform) {
            _rectTransform = rectTransform;
            _layers = new Dictionary<UILayer, RectTransform>();
            _layers[UILayer.WORLD] = CreateLayer("WORLD");
            _layers[UILayer.GUI] = CreateLayer("GUI");
            _layers[UILayer.WINDOW] = CreateLayer("WINDOW");
            _layers[UILayer.FLYTEXT] = CreateLayer("FLYTEXT");
            _layers[UILayer.DRAG] = CreateLayer("DRAG");
            _layers[UILayer.POPUP] = CreateLayer("POPUP");
        }
        
        private RectTransform CreateLayer(string name) {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.SetParent(_rectTransform);
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            return rectTransform;
        }
        
        public RectTransform GetLayer(UILayer layer)
        {
            return _layers[layer];
        }
    }
}