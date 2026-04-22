using System;
using Rukhanka.Toolbox;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public class ActiveAbilityButton : MonoBehaviour
    {
        public ConfigsLoader Configs;
        public Button Button;
        public RectTransform Bar;
        public Image Icon;
        
        private EntityManager _entityManager;
        private Entity _character;
        private uint _abilityID;
        
        public static void Create(Entity character)
        {
            var prefab = Resources.Load<GameObject>("UI/Prefabs/BattleHUD/Ability");
            var parent = FindAnyObjectByType<BattleHUD>().AbilityContainer;
            var ability = Instantiate(prefab, parent).GetComponent<ActiveAbilityButton>();
            ability.Initialize(character);
        }
        
        public void Initialize(Entity character)
        {
            _character = character;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _abilityID = _entityManager.GetComponentData<ActiveAbility>(_character).Value;
            Button.onClick.AddListener(OnActivateAbility);
            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
            Icon.sprite = GetIcon();
        }
        
        private void Update()
        {
            Bar.localScale = new Vector3(1, GetCooldownProgress(), 1);
        }

        private Sprite GetIcon()
        {
            var iconName = "Fire";
            foreach (var ability in Configs.Abilities.GetAll())
            {
                if (ability.AbilityID.CalculateHash32() == _abilityID)
                {
                    iconName = ability.Icon;
                    break;
                }
            }
            return Resources.Load<Sprite>($"Abilities/Icons/{iconName}");
        }

        private void OnActivateAbility()
        {
            if(IsAvailable())
                _entityManager.AddComponent<UseAbility>(_character);
        }

        private bool IsAvailable()
        {
            foreach (var ability in _entityManager.GetBuffer<Ability>(_character))
            {
                if (ability.Config.ID == _abilityID)
                    return ability.Cooldown >= ability.Config.Cooldown;
            }
            return false;
        }
        
        private float GetCooldownProgress()
        {
            foreach (var ability in _entityManager.GetBuffer<Ability>(_character))
            {
                if (ability.Config.ID == _abilityID)
                {
                    if(ability.Cooldown >= ability.Config.Cooldown) return 0;
                    return 1 - (ability.Cooldown / ability.Config.Cooldown);
                }
            }
            return 1;
        }
        
        private void OnDeadCharacter(DeadCharacterEvent evnt)
        {
            if (evnt.Character == _character) Destroy(gameObject);
        }
        
        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnActivateAbility);
            DeadCharacterEventSystem.OnExecute -= OnDeadCharacter;
        }
    }
}
