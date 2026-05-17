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
    public class BattleSkillViewModel : ViewModel<BattleWindowCharacterArgs>
    {
        private readonly EntityManager _entityManager;
        private readonly uint _skillID;

        public UnityAction Activate;
        public event Action Died;

        public Sprite Icon { get; }
        public string Title { get; }
        public bool IsDead { get; private set; }

        public BattleSkillViewModel(BattleWindowCharacterArgs args, IConfigs configs) : base(args)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _skillID = args.Config.GetSkill(SkillSlotType.Active);

            var characterData = FindCharacterData(configs, args.Config.ID);
            Icon = characterData?.PortraitImage;
            Title = characterData?.Name ?? string.Empty;
            Activate = OnActivateSkill;

            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
            AddDisposable(Disposable.Create(() => DeadCharacterEventSystem.OnExecute -= OnDeadCharacter));
        }

        public float GetHealthProgress()
        {
            if (!CanReadHealth()) return 0;

            var health = _entityManager.GetComponentData<Health>(Model.Character).Value;
            var healthMax = _entityManager.GetComponentData<HealthMax>(Model.Character).Value;
            return healthMax > 0 ? Mathf.Clamp01(health / healthMax) : 0;
        }

        public float GetCooldownProgress()
        {
            if (!IsCharacterAlive() || !_entityManager.HasComponent<Skill>(Model.Character)) return 0;

            foreach (var skill in _entityManager.GetBuffer<Skill>(Model.Character))
            {
                var config = skill.GetConfig();
                if (config.ID != _skillID) continue;
                if (skill.Cooldown >= config.Cooldown) return 1;
                return config.Cooldown > 0 ? Mathf.Clamp01(skill.Cooldown / config.Cooldown) : 1;
            }

            return 0;
        }

        private void OnActivateSkill()
        {
            if (!IsAvailable()) return;

            _entityManager.CreateFrameEntity(new ActivateSkillEvent
            {
                Character = Model.Character,
                SkillID = _skillID
            });
        }

        private bool IsAvailable()
        {
            if (!IsCharacterAlive() || !_entityManager.HasComponent<Skill>(Model.Character)) return false;

            foreach (var skill in _entityManager.GetBuffer<Skill>(Model.Character))
            {
                var config = skill.GetConfig();
                if (config.ID == _skillID)
                    return skill.Cooldown >= config.Cooldown;
            }

            return false;
        }

        private bool IsCharacterAlive()
        {
            return !IsDead && _entityManager.Exists(Model.Character);
        }

        private bool CanReadHealth()
        {
            return IsCharacterAlive()
                && _entityManager.HasComponent<Health>(Model.Character)
                && _entityManager.HasComponent<HealthMax>(Model.Character);
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
