using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace vikwhite
{
    public interface IRoomSelectionService : IUpdatable
    {
        void Clear();
        void Register(Collider collider, Room room);
    }

    public class RoomSelectionService : IRoomSelectionService
    {
        private const float ClickDragThreshold = 10f;

        private readonly IRoomWindow _roomWindow;
        private readonly Dictionary<Collider, Room> _roomsByCollider = new();
        private Vector2 _pointerDownPosition;
        private bool _isPointerDown;

        public RoomSelectionService(IRoomWindow roomWindow)
        {
            _roomWindow = roomWindow;
        }

        public void Clear()
        {
            _roomsByCollider.Clear();
            _isPointerDown = false;
        }

        public void Register(Collider collider, Room room)
        {
            _roomsByCollider.Add(collider, room);
        }

        public void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                BeginSelection(mouse.position.ReadValue());
                return;
            }

            if (mouse.leftButton.wasReleasedThisFrame)
                CompleteSelection(mouse.position.ReadValue());
        }

        private void BeginSelection(Vector2 screenPosition)
        {
            _isPointerDown = !IsPointerOverUi();
            _pointerDownPosition = screenPosition;
        }

        private void CompleteSelection(Vector2 screenPosition)
        {
            if (!_isPointerDown) return;

            _isPointerDown = false;
            if (IsPointerOverUi()) return;
            if ((screenPosition - _pointerDownPosition).sqrMagnitude > ClickDragThreshold * ClickDragThreshold) return;
            if (!TryGetRoom(screenPosition, out var room)) return;

            _roomWindow.ShowWindow(room);
        }

        private bool TryGetRoom(Vector2 screenPosition, out Room room)
        {
            room = null;
            var camera = Camera.main;
            if (camera == null) return false;

            var ray = camera.ScreenPointToRay(screenPosition);
            var nearestDistance = float.MaxValue;
            foreach (var hit in Physics.RaycastAll(
                         ray,
                         camera.farClipPlane,
                         Physics.DefaultRaycastLayers,
                         QueryTriggerInteraction.Collide))
            {
                if (hit.distance >= nearestDistance) continue;
                if (!_roomsByCollider.TryGetValue(hit.collider, out var hitRoom)) continue;

                room = hitRoom;
                nearestDistance = hit.distance;
            }

            return room != null;
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
