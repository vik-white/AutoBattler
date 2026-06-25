using Rukhanka.Toolbox;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public interface IBattleSquadPlacementService
    {
        void Begin();
        void End();
    }

    public class BattleSquadPlacementService : IBattleSquadPlacementService
    {
        private readonly ISquadService _squad;
        private readonly IBattleGridService _grid;
        private bool _isActive;

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
    }
}
