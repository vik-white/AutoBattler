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
        private const float AutoActivationAttemptInterval = 0.1f;

        private readonly EntityManager _entityManager;
        private readonly uint _skillID;
        private float _nextAutoActivationTime;

        public UnityAction Activate;
        public UnityAction OnActivate;
        public event Action Died;

        public Sprite RarityBG { get; }
        public Sprite RarityFrame { get; }
        public GameObject ImagePrefab { get; }
        public bool IsDead { get; private set; }

        public BattleSkillViewModel(BattleWindowCharacterArgs args, IConfigs configs) : base(args)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _skillID = args.Config.GetSkill(SkillSlotType.Active);

            var characterData = FindCharacterData(configs, args.Config.ID);
            ImagePrefab = characterData.HeadPrefab;
            RarityBG = configs.UI.Rarities[characterData.Rarity].BattleBG;
            RarityFrame = configs.UI.Rarities[characterData.Rarity].BattleFrame;
            Activate = OnActivateSkill;

            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
            StartedSkillEventSystem.OnExecute += OnStartedSkill;
            AddDisposable(Disposable.Create(() => DeadCharacterEventSystem.OnExecute -= OnDeadCharacter));
            AddDisposable(Disposable.Create(() => StartedSkillEventSystem.OnExecute -= OnStartedSkill));
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
                if (!SkillHandler.HasActivationsLeft(skill)) return 0;
                if (skill.Cooldown >= config.Cooldown) return 1;
                return config.Cooldown > 0 ? Mathf.Clamp01(skill.Cooldown / config.Cooldown) : 1;
            }

            return 0;
        }
        
        public int GetCooldown()
        {
            if (!IsCharacterAlive() || !_entityManager.HasComponent<Skill>(Model.Character)) return 0;
            foreach (var skill in _entityManager.GetBuffer<Skill>(Model.Character))
            {
                var config = skill.GetConfig();
                if (config.ID != _skillID) continue;
                if (!SkillHandler.HasActivationsLeft(skill)) return 0;
                return Mathf.CeilToInt(config.Cooldown - skill.Cooldown);
            }
            return 0;
        }

        public bool Exists()
        {
            return _entityManager.Exists(Model.Character);
        }

        public void AutoActivate()
        {
            if (UnityEngine.Time.unscaledTime < _nextAutoActivationTime) return;
            if (!IsAvailable()) return;

            _nextAutoActivationTime = UnityEngine.Time.unscaledTime + AutoActivationAttemptInterval;
            CreateActivateSkillEvent();
        }

        private void OnActivateSkill()
        {
            if (!IsAvailable()) return;

            CreateActivateSkillEvent();
        }

        private void CreateActivateSkillEvent()
        {
            _entityManager.CreateFrameEntity(new ActivateSkillEvent
            {
                Character = Model.Character,
                SkillID = _skillID
            });
        }

        private bool IsAvailable()
        {
            if (TimeSystem.IsPaused || !IsCharacterAlive() || !_entityManager.HasComponent<Skill>(Model.Character)) return false;
            if (HasSkillAnimationInProgress()) return false;

            foreach (var skill in _entityManager.GetBuffer<Skill>(Model.Character))
            {
                var config = skill.GetConfig();
                if (config.ID == _skillID)
                    return SkillHandler.HasActivationsLeft(skill) && skill.Cooldown >= config.Cooldown;
            }

            return false;
        }

        private bool HasSkillAnimationInProgress()
        {
            if (_entityManager.HasComponent<ActiveSkillAnimationLock>(Model.Character)) return true;
            if (!_entityManager.HasComponent<StarterSkill>(Model.Character)) return false;

            foreach (var pendingSkill in _entityManager.GetBuffer<StarterSkill>(Model.Character))
            {
                if (pendingSkill.WaitForAnimation && pendingSkill.Skill.Value.ID == _skillID) return true;
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

        private void OnStartedSkill(StartedSkillEvent evnt)
        {
            if (evnt.Character != Model.Character) return;
            if (evnt.Skill.Value.ID != _skillID) return;

            OnActivate?.Invoke();
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
            OnActivate = null;
            Died = null;
        }
    }
}
