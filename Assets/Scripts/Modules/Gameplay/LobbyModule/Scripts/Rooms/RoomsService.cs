using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRoomsService : IUpdatable
    {
        void Initialize();
    }
    
    public class RoomsService : IRoomsService
    {
        private const float ClickDragThreshold = 10f;

        private readonly IProfileService _profile;
        private readonly IConfigs _configs;
        private readonly IRoomWindow _roomWindow;
        private readonly IRoomFactory _roomFactory;
        private readonly List<Room> _rooms = new();
        private readonly Dictionary<Collider, Room> _roomsByCollider = new();
        private Vector2 _pointerDownPosition;
        private bool _isPointerDown;
        
        public RoomsService(IProfileService profile, IConfigs configs, IRoomWindow roomWindow, IRoomFactory roomFactory)
        {
            _profile = profile;
            _configs = configs;
            _roomWindow = roomWindow;
            _roomFactory = roomFactory;
        }

        public void Initialize()
        {
            _rooms.Clear();
            _roomsByCollider.Clear();

            var tavern = UnityEngine.Object.FindAnyObjectByType<TavernHierarchy>(FindObjectsInactive.Include);
            foreach (var room in tavern.Rooms)
            {
                room.Container.ClearChildren();
                var profileData = _profile.Data.Rooms.Find(e => e.Type == room.Type);
                var configData = _configs.Rooms.GetAll().Find(e => e.Type == room.Type);
                var roomModel = _roomFactory.Create(profileData.Type, profileData.Level);
                _rooms.Add(roomModel);
                _roomsByCollider.Add(room.Collider, roomModel);
                var roomGO = GameObject.Instantiate(configData.Prefab, room.Container);
                roomGO.transform.localPosition = Vector3.zero;
            }
        }

        public void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _isPointerDown = EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject();
                _pointerDownPosition = mouse.position.ReadValue();
                return;
            }

            if (!mouse.leftButton.wasReleasedThisFrame || !_isPointerDown) return;

            _isPointerDown = false;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if ((mouse.position.ReadValue() - _pointerDownPosition).sqrMagnitude > ClickDragThreshold * ClickDragThreshold) return;

            var camera = Camera.main;
            if (camera == null) return;

            var ray = camera.ScreenPointToRay(mouse.position.ReadValue());
            foreach (var hit in Physics.RaycastAll(ray, camera.farClipPlane, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                if (!_roomsByCollider.TryGetValue(hit.collider, out var room)) continue;
                _roomWindow.ShowWindow(room);
                return;
            }
        }
    }
}
