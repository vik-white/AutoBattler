using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using vikwhite.ECS;

namespace vikwhite
{
    public class HealthBar : MonoBehaviour
    {
        private const float HeadPadding = 0.15f;
        private const float WhiteHealthDelay = 0.5f;
        private const float WhiteHealthDecreaseSpeed = 1.5f;

        public RectTransform HealthProgressBar;
        public RectTransform HealthWhiteProgressBar;
        public RectTransform ShieldProgressBar;
        public GameObject ShieldBar;
        public Image HealthProgressBarImage;
        public Color SquadColor;
        public Color EnemyColor;
        
        private Entity _character;
        private CharacterConfigData _characterConfig;
        private EntityManager _entityManager;
        private bool _isHealthInitialized;
        private bool _isWhiteHealthDecreasing;
        private float _healthFill;
        private float _whiteHealthFill;
        private float _whiteHealthDelayLeft;

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
            HealthProgressBarImage.color = _entityManager.HasComponent<Enemy>(character) ? EnemyColor : SquadColor;
        }

        private void Update()
        {
            if(!_entityManager.Exists(_character)) return;
            var characterTransform = _entityManager.GetComponentData<LocalTransform>(_character);
            var position = characterTransform.Position + new float3(0, GetHeadOffset(characterTransform.Scale), 0);
            transform.position = Camera.main.WorldToScreenPoint(position);
            var health = _entityManager.GetComponentData<Health>(_character).Value;
            var healthMax = _entityManager.GetComponentData<HealthMax>(_character).Value;
            UpdateHealthBars(healthMax > 0 ? math.saturate(health / healthMax) : 0);
            
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

        private void UpdateHealthBars(float healthFill)
        {
            if (!_isHealthInitialized)
            {
                _healthFill = healthFill;
                _whiteHealthFill = healthFill;
                _isHealthInitialized = true;
                SetProgressBarScale(HealthProgressBar, _healthFill);
                SetProgressBarScale(HealthWhiteProgressBar, _whiteHealthFill);
                return;
            }

            var previousHealthFill = _healthFill;
            _healthFill = healthFill;
            SetProgressBarScale(HealthProgressBar, _healthFill);

            if (_healthFill >= _whiteHealthFill)
            {
                _whiteHealthDelayLeft = 0;
                _isWhiteHealthDecreasing = false;
                _whiteHealthFill = _healthFill;
                SetProgressBarScale(HealthWhiteProgressBar, _whiteHealthFill);
                return;
            }

            if (_healthFill < previousHealthFill && !_isWhiteHealthDecreasing && _whiteHealthDelayLeft <= 0)
                _whiteHealthDelayLeft = WhiteHealthDelay;

            if (_whiteHealthDelayLeft > 0)
            {
                _whiteHealthDelayLeft -= UnityEngine.Time.deltaTime;
                SetProgressBarScale(HealthWhiteProgressBar, _whiteHealthFill);
                return;
            }

            _isWhiteHealthDecreasing = true;
            _whiteHealthFill = Mathf.MoveTowards(_whiteHealthFill, _healthFill, WhiteHealthDecreaseSpeed * UnityEngine.Time.deltaTime);
            if (Mathf.Approximately(_whiteHealthFill, _healthFill))
                _isWhiteHealthDecreasing = false;

            SetProgressBarScale(HealthWhiteProgressBar, _whiteHealthFill);
        }

        private static void SetProgressBarScale(RectTransform progressBar, float value)
        {
            if (progressBar == null) return;
            progressBar.localScale = new Vector3(value, 1, 1);
        }

        private float GetHeadOffset(float scale)
        {
            var currentScale = math.max(scale, 0);
            var characterHeight = _characterConfig.ColliderHeight * currentScale / _characterConfig.Scale;
            return characterHeight + HeadPadding * currentScale;
        }
        
        private void OnDestroy()
        {
            DeadCharacterEventSystem.OnExecute -= OnDeadCharacter;
        }
    }
}
