using System;
using UniRx;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public class BattleHealthBarViewModel : ViewModel<BattleWindowCharacterModel>
    {
        private const float HeadPadding = 0.15f;

        private readonly EntityManager _entityManager;

        public event Action Died;

        public bool IsEnemy => Model.IsEnemy;
        public bool IsDead { get; private set; }

        public BattleHealthBarViewModel(BattleWindowCharacterModel model) : base(model)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
            AddDisposable(Disposable.Create(() => DeadCharacterEventSystem.OnExecute -= OnDeadCharacter));
        }

        public bool Exists()
        {
            return !IsDead && _entityManager.Exists(Model.Character);
        }

        public Vector3 GetHeadPosition()
        {
            if (!Exists() || !_entityManager.HasComponent<LocalTransform>(Model.Character)) return Vector3.zero;

            var characterTransform = _entityManager.GetComponentData<LocalTransform>(Model.Character);
            var position = characterTransform.Position + new float3(0, GetHeadOffset(characterTransform.Scale), 0);
            return new Vector3(position.x, position.y, position.z);
        }

        public float GetHealthFill()
        {
            if (!Exists()) return 0;
            if (!_entityManager.HasComponent<Health>(Model.Character) || !_entityManager.HasComponent<HealthMax>(Model.Character)) return 0;

            var health = _entityManager.GetComponentData<Health>(Model.Character).Value;
            var healthMax = _entityManager.GetComponentData<HealthMax>(Model.Character).Value;
            return healthMax > 0 ? math.saturate(health / healthMax) : 0;
        }

        public bool IsShieldVisible()
        {
            if (!Exists()) return false;
            if (!_entityManager.HasComponent<Shield>(Model.Character) || !_entityManager.HasComponent<ShieldMax>(Model.Character)) return false;

            var shield = _entityManager.GetComponentData<Shield>(Model.Character).Value;
            var shieldMax = _entityManager.GetComponentData<ShieldMax>(Model.Character).Value;
            return shield > 0 || shieldMax > 0;
        }

        public float GetShieldFill()
        {
            if (!Exists()) return 0;
            if (!_entityManager.HasComponent<Shield>(Model.Character) || !_entityManager.HasComponent<ShieldMax>(Model.Character)) return 0;

            var shield = _entityManager.GetComponentData<Shield>(Model.Character).Value;
            var shieldMax = _entityManager.GetComponentData<ShieldMax>(Model.Character).Value;
            return shieldMax > 0 ? math.saturate(shield / shieldMax) : 0;
        }

        private void OnDeadCharacter(DeadCharacterEvent evnt)
        {
            if (evnt.Character != Model.Character) return;

            IsDead = true;
            Died?.Invoke();
        }

        private float GetHeadOffset(float scale)
        {
            var currentScale = math.max(scale, 0);
            var characterHeight = Model.Config.ColliderHeight * currentScale / Model.Config.Scale;
            return characterHeight + HeadPadding * currentScale;
        }

        public override void Dispose()
        {
            base.Dispose();
            Died = null;
        }
    }
}
