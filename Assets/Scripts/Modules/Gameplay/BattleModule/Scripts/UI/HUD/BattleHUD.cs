using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using vikwhite.ECS;
using Time = UnityEngine.Time;

namespace vikwhite
{
    public class BattleHUD : MonoBehaviour
    {
        public Text FPS;
        public Button LobbyButton;
        public RectTransform AbilityContainer;
        
        private EntityManager _entityManager;

        public static void Show()
        {
            var canvas = FindAnyObjectByType<Canvas>().transform;
            var hud = Resources.Load<GameObject>("UI/Prefabs/BattleHUD/BattleHUD");
            Instantiate(hud, canvas).GetComponent<BattleHUD>().Initialize();
        }

        public static void Hide() => Destroy(FindAnyObjectByType<BattleHUD>().gameObject);

        private void Initialize()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            CreateCharacterEventSystem.OnExecute += OnCreateCharacter;
            LobbyButton.onClick.AddListener(() => DI.Resolve<IEnvironmentStateMachine>().SwitchState(EnvironmentType.Lobby));
        }

        private void Update()
        {
            FPS.text = $"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}";
        }

        private void OnCreateCharacter(CreateCharacterEvent evnt)
        {
            var character = _entityManager.GetComponentData<Character>(evnt.Character);
            var isEnemy = _entityManager.HasComponent<Enemy>(evnt.Character);
            var config = character.GetConfig();
            
            if(config.HealthBar) HealthBar.Create(evnt.Character, config);
            if(config.ActiveAbility != 0 && !isEnemy) ActiveAbilityButton.Create(evnt.Character);
        }

        private void OnDestroy()
        {
            CreateCharacterEventSystem.OnExecute -= OnCreateCharacter;
        }
    }
}
