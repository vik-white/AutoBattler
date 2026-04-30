using System;
using Rukhanka.Toolbox;
using TMPro;
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
        public RectTransform HealthBar;
        public RectTransform AbilityBar;
        public Image Fade;
        public Image Icon;
        public TMP_Text Title;
        
        private EntityManager _entityManager;
        private Entity _character;
        private ICharacterData _characterData;
        private uint _abilityID;
        private bool _isDead;
        
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
            var _characterID = _entityManager.GetComponentData<Character>(_character).Config.Value.ID;
            foreach (var characterData in Configs.Characters.GetAll())
            {
                if (characterData.ID.CalculateHash32() == _characterID)
                {
                    _characterData = characterData;
                    break;
                }
            }
            _abilityID = _entityManager.GetComponentData<ActiveAbility>(_character).Value;
            Button.onClick.AddListener(OnActivateAbility);
            DeadCharacterEventSystem.OnExecute += OnDeadCharacter;
            Icon.sprite = _characterData.PortraitImage;
            Fade.sprite = _characterData.PortraitImage;
            Title.text = _characterData.Name;
        }
        
        private void Update()
        {
            if(_isDead) return;
            HealthBar.localScale = new Vector3(GetHealthProgress(), 1, 1);
            AbilityBar.localScale = new Vector3(GetCooldownProgress(), 1, 1);
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
                var config = ability.GetConfig();
                if (config.ID == _abilityID)
                    return ability.Cooldown >= config.Cooldown;
            }
            return false;
        }
        
        private float GetHealthProgress()
        {
            var health = _entityManager.GetComponentData<Health>(_character).Value;
            var healthMax = _entityManager.GetComponentData<HealthMax>(_character).Value;
            return health / healthMax;
        }
        
        private float GetCooldownProgress()
        {
            foreach (var ability in _entityManager.GetBuffer<Ability>(_character))
            {
                var config = ability.GetConfig();
                if (config.ID == _abilityID)
                {
                    if(ability.Cooldown >= config.Cooldown) return 1;
                    return (ability.Cooldown / config.Cooldown);
                }
            }
            return 0;
        }
        
        private void OnDeadCharacter(DeadCharacterEvent evnt)
        {
            if (evnt.Character == _character)
            {
                Fade.gameObject.SetActive(true);
                HealthBar.gameObject.SetActive(false);
                AbilityBar.gameObject.SetActive(false);
                Button.onClick.RemoveListener(OnActivateAbility);
                _isDead = true;
            }
        }
        
        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnActivateAbility);
            DeadCharacterEventSystem.OnExecute -= OnDeadCharacter;
        }
    }
}
