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
        private CharacterConfig _characterConfig;
        private EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        public static void Create(Entity character, CharacterConfig characterConfig)
        {
            var prefab = Resources.Load<GameObject>("UI/HealthBar");
            var parent = FindAnyObjectByType<BattleHUD>().transform;
            var healthBar = Instantiate(prefab, parent).GetComponent<HealthBar>();
            healthBar.Initialize(character, characterConfig);
        }
        
        public void Initialize(Entity character, CharacterConfig characterConfig)
        {
            _character = character;
            _characterConfig = characterConfig;
            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
        }

        private void Update()
        {
            if(!_entityManager.Exists(_character)) return;
            var position = _entityManager.GetComponentData<LocalTransform>(_character).Position + new float3(0, 1.3f, 0);
            transform.position = Camera.main.WorldToScreenPoint(position);
            var health = _entityManager.GetComponentData<Health>(_character).Value;
            var healthMax = _characterConfig.Health;
            Bar.localScale = new Vector3(health / healthMax, 1, 1);
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