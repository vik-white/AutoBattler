using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace vikwhite
{
    public class UIDropContainer : MonoBehaviour, IDropHandler
    {
        public Transform Container;
        public Func<UIDrag, bool> OnAddElement;
        public Action<UIDrag> OnRemoveElement;

        public void OnDrop(PointerEventData eventData)
        {
            UIDrag.DragElement?.SetTargetContainer(this);
        }

        public void Add(UIDrag element)
        {
            element.transform.SetParent(Container);
        }
    }
}