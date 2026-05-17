using System;
using System.Collections.Generic;
using UniRx;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.ECS;
using Time = UnityEngine.Time;

namespace vikwhite
{
    public class BattleWindowViewModel : WindowViewModel
    {
        public readonly List<BattleHealthBarViewModel> HealthBars = new();
        public readonly List<BattleAbilityViewModel> Abilities = new();

        public event Action<BattleHealthBarViewModel> HealthBarCreated;
        public event Action<BattleAbilityViewModel> AbilityCreated;
        public event Action<BattleDamageFlyTextViewModel> DamageFlyTextCreated;

        public UnityAction OnLobby;
        public string FpsText => $"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}";

        private readonly IEnvironmentStateMachine _environmentStateMachine;

        public BattleWindowViewModel(IEnvironmentStateMachine environmentStateMachine)
        {
            _environmentStateMachine = environmentStateMachine;
            OnLobby = OpenLobby;

            CreateCharacterEventSystem.OnExecute += OnCreateCharacter;
            CreateDamageFlyTextEventSystem.OnExecute += OnCreateDamageFlyText;

            AddDisposable(Disposable.Create(() => CreateCharacterEventSystem.OnExecute -= OnCreateCharacter));
            AddDisposable(Disposable.Create(() => CreateDamageFlyTextEventSystem.OnExecute -= OnCreateDamageFlyText));
        }

        private void OpenLobby()
        {
            _environmentStateMachine.SwitchState(EnvironmentType.Lobby);
        }

        private void OnCreateCharacter(CreateCharacterEvent evnt)
        {
            if (!TryGetEntityManager(out var entityManager)) return;
            if (!entityManager.Exists(evnt.Character) || !entityManager.HasComponent<ECS.Character>(evnt.Character)) return;

            var character = entityManager.GetComponentData<ECS.Character>(evnt.Character);
            var config = character.GetConfig();
            var args = new BattleWindowCharacterArgs(evnt.Character, config, entityManager.HasComponent<Enemy>(evnt.Character));

            if (config.HealthBar)
            {
                var healthBar = CreateViewModel<BattleHealthBarViewModel, BattleWindowCharacterArgs>(args);
                HealthBars.Add(healthBar);
                AddDisposable(healthBar);
                HealthBarCreated?.Invoke(healthBar);
            }

            if (config.GetSkill(SkillSlotType.Active) != 0 && !args.IsEnemy)
            {
                var ability = CreateViewModel<BattleAbilityViewModel, BattleWindowCharacterArgs>(args);
                Abilities.Add(ability);
                AddDisposable(ability);
                AbilityCreated?.Invoke(ability);
            }
        }

        private void OnCreateDamageFlyText(CreateDamageFlyTextEvent evnt)
        {
            var args = new BattleDamageFlyTextArgs(
                new Vector3(evnt.Position.x, evnt.Position.y, evnt.Position.z),
                evnt.Damage,
                evnt.IsEnemyTarget,
                evnt.IsCrit);

            var flyText = CreateViewModel<BattleDamageFlyTextViewModel, BattleDamageFlyTextArgs>(args);
            DamageFlyTextCreated?.Invoke(flyText);
        }

        private static bool TryGetEntityManager(out EntityManager entityManager)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null)
            {
                entityManager = default;
                return false;
            }

            entityManager = world.EntityManager;
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnLobby = null;
            HealthBarCreated = null;
            AbilityCreated = null;
            DamageFlyTextCreated = null;
        }
    }
}
