using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public string ID;
        public static UIDrag DragElement;
        [HideInInspector] public UIDropContainer SourceContainer;
        [HideInInspector] public UIDropContainer TargetContainer;
        private CanvasGroup _canvasGroup;
        protected bool _isDraggable = true;
        protected bool _isDragging = false;
        private IUIRoot _uiRoot;
        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
        
        private void Awake()
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _uiRoot = DI.Resolve<IUIRoot>();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!_isDraggable) return;
            _isDragging = true;
            DragElement = this;
            TargetContainer = null;
            _canvasGroup.blocksRaycasts = false;
            SourceContainer = transform.GetComponentInParent<UIDropContainer>();
            transform.SetParent(_uiRoot.GetLayer(UILayer.DRAG));
            SourceContainer.OnRemoveElement?.Invoke(this);
            RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var mouseCanvasPosition = Camera.main.ScreenToViewportPoint(mousePosition) * _uiRoot.CanvasSize;
            RectTransform.anchoredPosition = mouseCanvasPosition - _uiRoot.CanvasCenter;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDraggable) return;
            _canvasGroup.blocksRaycasts = true;
            if (TargetContainer == null)
            {
                transform.SetParent(SourceContainer.Container);
                SourceContainer.OnAddElement?.Invoke(this);
            }
            else
            {
                if (SourceContainer == TargetContainer)
                {
                    transform.SetParent(SourceContainer.Container);
                    SourceContainer.OnAddElement?.Invoke(this);
                }
                else
                {
                    transform.SetParent(TargetContainer.Container);
                    if (TargetContainer.OnAddElement == null || !TargetContainer.OnAddElement.Invoke(this))
                    {
                        transform.SetParent(SourceContainer.Container);
                        SourceContainer.OnAddElement?.Invoke(this);
                    }
                }
            }

            TargetContainer = null;
        }

        public void SetTargetContainer(UIDropContainer targetContainer)
        {
            TargetContainer = targetContainer;
        }

        public bool IsSource<T>() where T : class
        {
            return SourceContainer.GetComponentInParent<T>() != null;
        }
    }
}