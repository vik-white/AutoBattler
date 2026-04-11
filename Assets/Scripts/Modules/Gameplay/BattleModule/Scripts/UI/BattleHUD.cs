using UnityEngine;
using UnityEngine.UI;
using vikwhite.ECS;
using Time = UnityEngine.Time;

namespace vikwhite
{
    public class BattleHUD : MonoBehaviour
    {
        public Text FPS;
        public RectTransform AbilityContainer;

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
        }

        private void Update()
        {
            FPS.text = $"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}";
        }

        private void OnCreateCharacter(CreateCharacterEvent evnt)
        {
            HealthBar.Create(evnt.Character);
            ActiveAbilityButton.Create(evnt.Character);
        }

        private void OnDestroy()
        {
            CreateCharacterEventSystem.OnExecute -= OnCreateCharacter;
        }
    }
}