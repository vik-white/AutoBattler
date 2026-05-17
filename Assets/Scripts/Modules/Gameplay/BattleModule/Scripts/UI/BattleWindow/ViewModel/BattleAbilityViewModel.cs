using System;
using Rukhanka.Toolbox;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Extensions;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleAbilityViewModel : ViewModel<BattleWindowCharacterModel>
    {
        private readonly EntityManager _entityManager;
        private readonly uint _abilityID;

        public UnityAction Activate;
        public event Action Died;

        public Sprite Icon { get; }
        public string Title { get; }
        public bool IsDead { get; private set; }

        public BattleAbilityViewModel(BattleWindowCharacterModel model, IConfigs configs) : base(model)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _abilityID = model.Config.GetSkill(SkillSlotType.Active);

            var characterData = FindCharacterData(configs, model.Config.ID);
            Icon = characterData?.PortraitImage;
            Title = characterData?.Name ?? string.Empty;
            Activate = OnActivateAbility;

            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
            AddDisposable(Disposable.Create(() => DeadCharacterEventSystem.OnExecute -= OnDeadCharacter));
        }

        public float GetHealthProgress()
        {
            if (!IsCharacterAlive()) return 0;
            if (!_entityManager.HasComponent<Health>(Model.Character) || !_entityManager.HasComponent<HealthMax>(Model.Character)) return 0;

            var health = _entityManager.GetComponentData<Health>(Model.Character).Value;
            var healthMax = _entityManager.GetComponentData<HealthMax>(Model.Character).Value;
            return healthMax > 0 ? Mathf.Clamp01(health / healthMax) : 0;
        }

        public float GetCooldownProgress()
        {
            if (!IsCharacterAlive() || !_entityManager.HasComponent<Skill>(Model.Character)) return 0;

            foreach (var ability in _entityManager.GetBuffer<Skill>(Model.Character))
            {
                var config = ability.GetConfig();
                if (config.ID != _abilityID) continue;
                if (ability.Cooldown >= config.Cooldown) return 1;
                return config.Cooldown > 0 ? Mathf.Clamp01(ability.Cooldown / config.Cooldown) : 1;
            }

            return 0;
        }

        private void OnActivateAbility()
        {
            if (!IsAvailable()) return;

            _entityManager.CreateFrameEntity(new ActivateSkillEvent
            {
                Character = Model.Character,
                SkillID = _abilityID
            });
        }

        private bool IsAvailable()
        {
            if (!IsCharacterAlive() || !_entityManager.HasComponent<Skill>(Model.Character)) return false;

            foreach (var ability in _entityManager.GetBuffer<Skill>(Model.Character))
            {
                var config = ability.GetConfig();
                if (config.ID == _abilityID)
                    return ability.Cooldown >= config.Cooldown;
            }

            return false;
        }

        private bool IsCharacterAlive()
        {
            return !IsDead && _entityManager.Exists(Model.Character);
        }

        private void OnDeadCharacter(DeadCharacterEvent evnt)
        {
            if (evnt.Character != Model.Character) return;

            IsDead = true;
            Died?.Invoke();
        }

        private static ICharacterData FindCharacterData(IConfigs configs, uint characterID)
        {
            foreach (var characterData in configs.Characters.GetAll())
            {
                if (characterData.ID.CalculateHash32() == characterID)
                    return characterData;
            }

            return null;
        }

        public override void Dispose()
        {
            base.Dispose();
            Activate = null;
            Died = null;
        }
    }
}
