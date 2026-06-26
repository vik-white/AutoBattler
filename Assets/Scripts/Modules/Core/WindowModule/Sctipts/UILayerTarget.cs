using System;
using UnityEngine;

namespace vikwhite
{
    public class UILayerTarget : MonoBehaviour
    {
        [SerializeField] private UILayer layer = UILayer.POPUP;
        [SerializeField] private bool worldPositionStays;

        private void OnEnable()
        {
            MoveToLayer();
        }

        private void Start()
        {
            MoveToLayer();
        }

        private void MoveToLayer()
        {
            try
            {
                var target = DI.Resolve<IUIRoot>().GetLayer(layer);
                if (target == null || transform.parent == target)
                    return;

                transform.SetParent(target, worldPositionStays);
                transform.SetAsLastSibling();

                if (!worldPositionStays && transform is RectTransform rectTransform)
                {
                    rectTransform.localScale = Vector3.one;
                    rectTransform.localRotation = Quaternion.identity;
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to move {name} to UI layer {layer}: {exception.Message}", this);
            }
        }
    }
}
