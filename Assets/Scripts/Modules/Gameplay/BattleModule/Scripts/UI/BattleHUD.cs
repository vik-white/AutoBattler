using System.Collections.Generic;
using Unity.Collections;
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

        private EntityManager _entityManager;
        private EntityQuery _createCharacterQuery;

        public static void Show()
        {
            var canvas = FindAnyObjectByType<Canvas>().transform;
            var hud = Resources.Load<GameObject>("UI/BattleHUD");
            Instantiate(hud, canvas).GetComponent<BattleHUD>().Initialize();
        }

        public static void Hide()
        {
            Destroy(FindAnyObjectByType<BattleHUD>().gameObject);
        }

        private void Initialize()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _createCharacterQuery = _entityManager.CreateEntityQuery(typeof(CreateCharacter));
        }

        private void Update()
        {
            FPS.text = $"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}";
            foreach (var createCharacter in _createCharacterQuery.ToComponentDataArray<CreateCharacter>(Allocator.Temp))
            {
                OnCreateCharacter(createCharacter);
            }
        }

        private void OnCreateCharacter(CreateCharacter createCharacter)
        {
            if (!createCharacter.IsEnemy)
            {
                var prefab = Resources.Load<GameObject>("UI/HealthBar");
                var healthBar = Instantiate(prefab, transform).GetComponent<HealthBar>();
                healthBar.Initialize(createCharacter.Entity);
            }
        }
    }
}