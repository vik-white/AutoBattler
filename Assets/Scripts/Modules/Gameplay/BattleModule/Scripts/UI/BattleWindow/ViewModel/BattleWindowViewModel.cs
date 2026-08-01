using System;
using System.Collections.Generic;
using Unity.Collections;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Extensions;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleWindowViewModel : WindowViewModel
    {
        public readonly List<BattleHealthBarViewModel> HealthBars = new();
        public readonly List<BattleSkillViewModel> Skills = new();

        public event Action<BattleHealthBarViewModel> HealthBarCreated;
        public event Action<BattleSkillViewModel> SkillCreated;
        public event Action<BattleDamageFlyTextViewModel> DamageFlyTextCreated;

        private readonly ReactiveProperty<bool> _autoUseSkills;
        public UnityAction OnQuickVictory;
        public UnityAction OnPause;
        public UnityAction OnToggleAutoUseSkills;
        public IReadOnlyReactiveProperty<bool> AutoUseSkills => _autoUseSkills;
        public string FpsText => $"FPS: {(TimeSystem.UnscaledDeltaTime > 0f ? Mathf.RoundToInt(1f / TimeSystem.UnscaledDeltaTime) : 0)}";
        public int PlayerMight;
        public int EnemyMight;

        private readonly IStateMachine<IBattleState> _battleStateMachine;
        private readonly IProfileService _profile;
        private readonly HashSet<Entity> _characters = new();
        private bool _quickVictoryRequested;

        public BattleWindowViewModel(IStateMachine<IBattleState> battleStateMachine, ISquadService squad, IProfileService profile)
        {
            _battleStateMachine = battleStateMachine;
            _profile = profile;
            _autoUseSkills = new ReactiveProperty<bool>(_profile.Data.AutoUseSkills);
            OnQuickVictory = QuickVictory;
            OnPause = TogglePause;
            OnToggleAutoUseSkills = ToggleAutoUseSkills;
            PlayerMight = squad.PlayerMight.Value;
            EnemyMight = squad.EnemyMight.Value;

            CreateCharacterEventSystem.OnExecute += OnCreateCharacter;
            CreateDamageFlyTextEventSystem.OnExecute += OnCreateDamageFlyText;
            AddDisposable(Observable.EveryUpdate().Subscribe(_ => AutoUseReadySkills()));
            AddDisposable(_autoUseSkills);
            AddExistingCharacters();
        }

        private static void TogglePause()
        {
            TimeSystem.TogglePause();
        }

        private void ToggleAutoUseSkills()
        {
            _autoUseSkills.Value = !_autoUseSkills.Value;
            _profile.SetAutoUseSkills(_autoUseSkills.Value);
            _profile.Save();
        }

        private void AutoUseReadySkills()
        {
            if (!_autoUseSkills.Value) return;

            for (var i = 0; i < Skills.Count; i++)
                Skills[i].AutoActivate();
        }

        private void QuickVictory()
        {
            if (_quickVictoryRequested) return;
            _quickVictoryRequested = true;
            TimeSystem.SetPaused(false);
            ECSWorld.SetManagedEnabled<BattleSystemGroup>(false);
            _battleStateMachine.SwitchState<IBattleVictoryState>();
        }

        private void OnCreateCharacter(CreateCharacterEvent evnt)
        {
            if (!World.DefaultGameObjectInjectionWorld.TryGetEntityManager(out var entityManager)) return;
            AddCharacter(entityManager, evnt.Character);
        }

        private void AddExistingCharacters()
        {
            if (!World.DefaultGameObjectInjectionWorld.TryGetEntityManager(out var entityManager)) return;

            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ECS.Character>());
            var characters = query.ToEntityArray(Allocator.Temp);
            foreach (var character in characters)
                AddCharacter(entityManager, character);

            characters.Dispose();
            query.Dispose();
        }

        private void AddCharacter(EntityManager entityManager, Entity characterEntity)
        {
            if (!entityManager.Exists(characterEntity)
                || !entityManager.HasComponent<ECS.Character>(characterEntity)
                || entityManager.HasComponent<Dead>(characterEntity))
                return;
            if (!_characters.Add(characterEntity)) return;

            var character = entityManager.GetComponentData<ECS.Character>(characterEntity);
            var config = character.GetConfig();
            var args = new BattleWindowCharacterArgs(characterEntity, config, entityManager.HasComponent<Enemy>(characterEntity));

            if (config.HealthBar)
            {
                var healthBar = CreateViewModel<BattleHealthBarViewModel, BattleWindowCharacterArgs>(args);
                HealthBars.Add(healthBar);
                AddDisposable(healthBar);
                HealthBarCreated?.Invoke(healthBar);
            }

            if (config.GetSkill(SkillSlotType.Active) != 0
                && !args.IsEnemy
                && entityManager.HasComponent<SquadSelection>(characterEntity))
            {
                var skill = CreateViewModel<BattleSkillViewModel, BattleWindowCharacterArgs>(args);
                Skills.Add(skill);
                AddDisposable(skill);
                SkillCreated?.Invoke(skill);
            }
        }

        private void OnCreateDamageFlyText(CreateDamageFlyTextEvent evnt)
        {
            var args = new BattleDamageFlyTextArgs(evnt.Position, evnt.Damage, evnt.IsEnemyTarget, evnt.IsCrit);
            var flyText = CreateViewModel<BattleDamageFlyTextViewModel, BattleDamageFlyTextArgs>(args);
            DamageFlyTextCreated?.Invoke(flyText);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnQuickVictory = null;
            OnPause = null;
            OnToggleAutoUseSkills = null;
            HealthBarCreated = null;
            SkillCreated = null;
            DamageFlyTextCreated = null;
            CreateCharacterEventSystem.OnExecute -= OnCreateCharacter;
            CreateDamageFlyTextEventSystem.OnExecute -= OnCreateDamageFlyText;
        }
    }
}
