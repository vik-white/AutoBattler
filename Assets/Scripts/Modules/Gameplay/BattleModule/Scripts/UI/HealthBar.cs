using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public class HealthBar : MonoBehaviour
    {
        public RectTransform Bar;
        private Entity _character;
        private EntityManager _entityManager;

        public static void Create(Entity character)
        {
            var prefab = Resources.Load<GameObject>("UI/HealthBar");
            var parent = FindAnyObjectByType<BattleHUD>().transform;
            var healthBar = Instantiate(prefab, parent).GetComponent<HealthBar>();
            healthBar.Initialize(character);
        }
        
        public void Initialize(Entity character)
        {
            _character = character;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
        }

        private void Update()
        {
            if(!_entityManager.Exists(_character)) return;
            var position = _entityManager.GetComponentData<LocalTransform>(_character).Position + new float3(0, 1.3f, 0);
            transform.position = Camera.main.WorldToScreenPoint(position);
            var health = _entityManager.GetComponentData<Health>(_character).Value;
            Bar.localScale = new Vector3(health / 500f, 1, 1);
        }

        private void OnDeadCharacter(DeadCharacterEvent evnt)
        {
            if (evnt.Character == _character) Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            DeadCharacterEventSystem.OnExecute -= OnDeadCharacter;
        }
    }
}