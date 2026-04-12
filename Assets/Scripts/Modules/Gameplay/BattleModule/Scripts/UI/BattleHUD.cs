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
        
        private EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        public static void Show()
        {
            var canvas = FindAnyObjectByType<Canvas>().transform;
            var hud = Resources.Load<GameObject>("UI/BattleHUD");
            Instantiate(hud, canvas).GetComponent<BattleHUD>().Initialize();
        }

        public static void Hide() => Destroy(FindAnyObjectByType<BattleHUD>().gameObject);

        private void Initialize()
        {
            CreateCharacterEventSystem.OnExecute += OnCreateCharacter;
            LobbyButton.onClick.AddListener(() => DI.Resolve<IEnvironmentStateMachine>().SwitchState(EnvironmentType.Lobby));
        }

        private void Update()
        {
            FPS.text = $"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}";
        }

        private void OnCreateCharacter(CreateCharacterEvent evnt)
        {
            var id = _entityManager.GetComponentData<Character>(evnt.Character).ID;
            var isEnemy = _entityManager.HasComponent<Enemy>(evnt.Character);
            var entityCharacterConfig = _entityManager.CreateEntityQuery(typeof(CharacterConfig)).GetSingletonEntity();
            var config = _entityManager.GetBuffer<CharacterConfig>(entityCharacterConfig).Get(id);
            
            if(config.HealthBar) HealthBar.Create(evnt.Character, config);
            if(config.ActiveAbility != AbilityID.None && !isEnemy) ActiveAbilityButton.Create(evnt.Character);
        }

        private void OnDestroy()
        {
            CreateCharacterEventSystem.OnExecute -= OnCreateCharacter;
        }
    }
}