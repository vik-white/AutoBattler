using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using vikwhite.ECS;

namespace vikwhite
{
    public class HealthBar : MonoBehaviour
    {
        public RectTransform HealthProgressBar;
        public RectTransform ShieldProgressBar;
        public GameObject ShieldBar;
        private Entity _character;
        private CharacterConfigData _characterConfig;
        private EntityManager _entityManager;

        public static void Create(Entity character, CharacterConfigData characterConfig)
        {
            var prefab = Resources.Load<GameObject>("UI/Prefabs/BattleHUD/HealthBar");
            var parent = FindAnyObjectByType<BattleHUD>().transform;
            var healthBar = Instantiate(prefab, parent).GetComponent<HealthBar>();
            healthBar.Initialize(character, characterConfig);
        }
        
        public void Initialize(Entity character, CharacterConfigData characterConfig)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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
            var healthMax = _entityManager.GetComponentData<HealthMax>(_character).Value;
            HealthProgressBar.localScale = new Vector3(health / healthMax, 1, 1);
            
            var shield = _entityManager.GetComponentData<Shield>(_character).Value;
            var shieldMax = _entityManager.GetComponentData<ShieldMax>(_character).Value;
            var isShowShield = shield > 0 || shieldMax > 0;
            ShieldBar.SetActive(isShowShield);
            if(isShowShield) ShieldProgressBar.localScale = new Vector3(shield / shieldMax, 1, 1);
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
