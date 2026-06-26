using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utilities.Extensions;
using vikwhite.Data;
using vikwhite.ECS;
using BattleCharacter = vikwhite.ECS.Character;

namespace vikwhite
{
    public interface IBattleSquadPlacementService
    {
        void Begin();
        void End();
    }

    public class BattleSquadPlacementService : IBattleSquadPlacementService, IUpdatable
    {
        private const float MinimumPickRadius = 0.45f;
        private const float PickRadiusPadding = 0.25f;

        private readonly ISquadService _squad;
        private readonly IBattleGridService _grid;
        private bool _isActive;
        private Entity _draggedEntity = Entity.Null;
        private float3 _dragStartPosition;
        private int2 _dragStartCell;
        private bool _hasDragStartCell;

        public BattleSquadPlacementService(ISquadService squad, IBattleGridService grid)
        {
            _squad = squad;
            _grid = grid;
        }

        public void Begin()
        {
            if (_isActive) return;

            _isActive = true;
            _grid.SetVisible(true);
            _squad.CharacterSelected += PlaceCharacter;
            _squad.CharacterDeselected += RemoveCharacter;
        }

        public void End()
        {
            if (!_isActive) return;

            _isActive = false;
            _grid.SetVisible(false);
            _squad.CharacterSelected -= PlaceCharacter;
            _squad.CharacterDeselected -= RemoveCharacter;
            ClearDrag();
        }

        public void Update()
        {
            if (!_isActive) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            var screenPosition = mouse.position.ReadValue();
            if (mouse.leftButton.wasPressedThisFrame)
                TryBeginDrag(screenPosition);

            if (_draggedEntity == Entity.Null) return;

            if (mouse.leftButton.isPressed)
                Drag(screenPosition);

            if (mouse.leftButton.wasReleasedThisFrame)
                EndDrag(screenPosition);
        }

        private static void PlaceCharacter(Character character, int slot)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;

            var characterID = character.ID.CalculateHash32();
            world.EntityManager.CreateFrameEntity(new CreateCharacter
            {
                ID = characterID,
                SquadCharacterID = characterID,
                SquadSlot = slot,
                Level = character.Level.Value,
                Stars = character.Stars.Value,
                SkillLevel = character.GetSkillLevel(SkillSlotType.Active),
                Position = InitializeSquadSystem.GetPosition(slot)
            });
        }

        private static void RemoveCharacter(Character character, int slot)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;

            var entityManager = world.EntityManager;
            var characterID = character.ID.CalculateHash32();
            DestroyCreationRequests(entityManager, characterID, slot);
            DestroySpawnedCharacters(entityManager, characterID, slot);
        }

        private static void DestroyCreationRequests(EntityManager entityManager, uint characterID, int slot)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<CreateCharacter>());
            var entities = query.ToEntityArray(Allocator.Temp);
            var requests = query.ToComponentDataArray<CreateCharacter>(Allocator.Temp);

            for (var i = 0; i < requests.Length; i++)
            {
                if (requests[i].SquadCharacterID == characterID && requests[i].SquadSlot == slot)
                    entityManager.DestroyEntity(entities[i]);
            }

            requests.Dispose();
            entities.Dispose();
            query.Dispose();
        }

        private static void DestroySpawnedCharacters(EntityManager entityManager, uint characterID, int slot)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<SquadSelection>());
            var entities = query.ToEntityArray(Allocator.Temp);
            var selections = query.ToComponentDataArray<SquadSelection>(Allocator.Temp);

            for (var i = 0; i < selections.Length; i++)
            {
                if (selections[i].CharacterID != characterID || selections[i].Slot != slot) continue;
                if (!entityManager.HasComponent<Destroy>(entities[i]))
                    entityManager.AddComponent<Destroy>(entities[i]);
            }

            selections.Dispose();
            entities.Dispose();
            query.Dispose();
        }

        private void TryBeginDrag(Vector2 screenPosition)
        {
            if (IsPointerOverUi()) return;
            if (!TryGetMouseGroundPosition(screenPosition, out var groundPosition)) return;
            if (!TryGetNearestDraggableCharacter(groundPosition, out var entity)) return;

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _draggedEntity = entity;
            _dragStartPosition = entityManager.GetComponentData<LocalTransform>(entity).Position;
            _hasDragStartCell = BattleSquadGridPositions.TryGetNearestCell(
                _dragStartPosition,
                BattleSquadGridPositions.DropSnapDistance,
                out _dragStartCell,
                out _);
            SetEntityPosition(entityManager, entity, groundPosition);
        }

        private void Drag(Vector2 screenPosition)
        {
            if (!TryGetActiveEntityManager(out var entityManager)) return;
            if (!TryGetMouseGroundPosition(screenPosition, out var groundPosition)) return;

            SetEntityPosition(entityManager, _draggedEntity, groundPosition);
        }

        private void EndDrag(Vector2 screenPosition)
        {
            if (!TryGetActiveEntityManager(out var entityManager)) return;

            if (!TryGetMouseGroundPosition(screenPosition, out var groundPosition) ||
                !BattleSquadGridPositions.TryGetNearestCell(groundPosition, out var targetCell, out var targetPosition))
            {
                ReturnDraggedCharacter(entityManager);
                return;
            }

            if (_hasDragStartCell && BattleSquadGridPositions.SameCell(_dragStartCell, targetCell))
            {
                SetEntityPosition(entityManager, _draggedEntity, targetPosition);
                ClearDrag();
                return;
            }

            if (TryGetCellOccupant(entityManager, targetCell, _draggedEntity, out var occupant))
            {
                SetEntityPosition(entityManager, _draggedEntity, targetPosition);
                SetEntityPosition(entityManager, occupant, GetSwapReturnPosition());
                ClearDrag();
                return;
            }

            SetEntityPosition(entityManager, _draggedEntity, targetPosition);
            ClearDrag();
        }

        private float3 GetSwapReturnPosition()
        {
            return _hasDragStartCell
                ? BattleSquadGridPositions.GetWorldPosition(_dragStartCell)
                : _dragStartPosition;
        }

        private void ReturnDraggedCharacter(EntityManager entityManager)
        {
            SetEntityPosition(entityManager, _draggedEntity, _dragStartPosition);
            ClearDrag();
        }

        private void ClearDrag()
        {
            _draggedEntity = Entity.Null;
            _dragStartPosition = default;
            _dragStartCell = default;
            _hasDragStartCell = false;
        }

        private bool TryGetActiveEntityManager(out EntityManager entityManager)
        {
            entityManager = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || _draggedEntity == Entity.Null) return false;

            entityManager = world.EntityManager;
            if (entityManager.Exists(_draggedEntity) && entityManager.HasComponent<LocalTransform>(_draggedEntity))
                return true;

            ClearDrag();
            return false;
        }

        private static bool TryGetNearestDraggableCharacter(float3 groundPosition, out Entity entity)
        {
            entity = Entity.Null;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;

            var entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SquadSelection>(),
                ComponentType.ReadOnly<BattleCharacter>(),
                ComponentType.ReadOnly<LocalTransform>());
            var entities = query.ToEntityArray(Allocator.Temp);
            var transforms = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
            var characters = query.ToComponentDataArray<BattleCharacter>(Allocator.Temp);

            var nearestIndex = -1;
            var nearestDistanceSq = float.MaxValue;
            var groundXZ = groundPosition.xz;

            for (var i = 0; i < entities.Length; i++)
            {
                if (entityManager.HasComponent<Destroy>(entities[i])) continue;

                var pickRadius = math.max(MinimumPickRadius, characters[i].GetConfig().ColliderRadius + PickRadiusPadding);
                var distanceSq = math.distancesq(groundXZ, transforms[i].Position.xz);
                if (distanceSq > pickRadius * pickRadius || distanceSq >= nearestDistanceSq) continue;

                nearestDistanceSq = distanceSq;
                nearestIndex = i;
            }

            if (nearestIndex >= 0)
                entity = entities[nearestIndex];

            characters.Dispose();
            transforms.Dispose();
            entities.Dispose();
            query.Dispose();

            return entity != Entity.Null;
        }

        private static bool TryGetCellOccupant(EntityManager entityManager, int2 cell, Entity ignoredEntity, out Entity occupant)
        {
            occupant = Entity.Null;

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SquadSelection>(),
                ComponentType.ReadOnly<LocalTransform>());
            var entities = query.ToEntityArray(Allocator.Temp);
            var transforms = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);

            for (var i = 0; i < entities.Length; i++)
            {
                if (entities[i] == ignoredEntity || entityManager.HasComponent<Destroy>(entities[i])) continue;
                if (!BattleSquadGridPositions.TryGetNearestCell(transforms[i].Position, out var occupiedCell, out _)) continue;
                if (!BattleSquadGridPositions.SameCell(occupiedCell, cell)) continue;

                occupant = entities[i];
                break;
            }

            transforms.Dispose();
            entities.Dispose();
            query.Dispose();

            return occupant != Entity.Null;
        }

        private static void SetEntityPosition(EntityManager entityManager, Entity entity, float3 position)
        {
            if (!entityManager.Exists(entity) || !entityManager.HasComponent<LocalTransform>(entity)) return;

            position.y = 0f;

            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            transform.Position = position;
            entityManager.SetComponentData(entity, transform);

            if (entityManager.HasComponent<PreviousPosition>(entity))
                entityManager.SetComponentData(entity, new PreviousPosition { Value = position });

            if (entityManager.HasComponent<MoveDistance>(entity))
                entityManager.SetComponentData(entity, new MoveDistance { Value = 0f });

            if (entityManager.HasComponent<PathAvoidanceState>(entity))
                entityManager.SetComponentData(entity, default(PathAvoidanceState));
        }

        private static bool TryGetMouseGroundPosition(Vector2 screenPosition, out float3 groundPosition)
        {
            groundPosition = default;

            var camera = Camera.main;
            if (camera == null) return false;

            var ray = camera.ScreenPointToRay(screenPosition);
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (!groundPlane.Raycast(ray, out var distance)) return false;

            groundPosition = ray.GetPoint(distance);
            return true;
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
