using System;
using System.Collections.Generic;
using UniRx;
using vikwhite.Data;
using vikwhite.ECS;

namespace vikwhite
{
    public class Character
    {
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        
        private string _id;
        private ICharacterData _characterData;
        private ReactiveProperty<float> _health;
        private ReactiveProperty<float> _attack;
        private ReactiveProperty<int> _level;
        private ReactiveProperty<int> _shards;
        private ReactiveProperty<int> _stars;
        private ReactiveProperty<int> _skillLevel;
        private readonly List<CharacterSkill> _skills = new();
        private readonly Dictionary<string, CharacterSkill> _skillsByID = new();
        private readonly Dictionary<SkillSlotType, CharacterSkill> _skillsBySlot = new();
        private CharacterUpgrade _upgrade;

        public string ID => _id;
        public ICharacterData Config => _characterData;
        public IReadOnlyReactiveProperty<float> Health => _health;
        public IReadOnlyReactiveProperty<float> Attack => _attack;
        public IReadOnlyReactiveProperty<int> Level => _level;
        public IReadOnlyReactiveProperty<int> Shards => _shards;
        public IReadOnlyReactiveProperty<int> Stars => _stars;
        public IReadOnlyReactiveProperty<int> SkillLevel => _skillLevel;
        public IReadOnlyList<CharacterSkill> Skills => _skills;

        public Character(IConfigs configs, IEventDispatcher dispatcher)
        {
            _configs = configs;
            _dispatcher = dispatcher;
        }
        
        public void Initialize(string id, int level, int shards, int stars, IReadOnlyList<SkillData> skills)
        {
            _id = id;
            _characterData = _configs.Characters.Get(id);
            _level = new ReactiveProperty<int>(level);
            _shards = new ReactiveProperty<int>(shards);
            _stars = new ReactiveProperty<int>(stars);
            _skillLevel = new ReactiveProperty<int>(1);
            InitializeSkills(skills);
            _upgrade = CreateUpgrade();
            _health = new ReactiveProperty<float>(GetHealth());
            _attack = new ReactiveProperty<float>(GetAttack());
            _level.Skip(1).Subscribe(value => { _dispatcher.Dispatch(new ChangeCharacterLevelEvent(_id, value)); CalculateStats(); });
            _shards.Skip(1).Subscribe(value => { _dispatcher.Dispatch(new ChangeCharacterShardEvent(_id, value)); CalculateStats(); });
            _stars.Skip(1).Subscribe(value => { _dispatcher.Dispatch(new ChangeCharacterStarsEvent(_id, value)); CalculateStats(); });
        }

        private CharacterUpgrade CreateUpgrade() => new (
            _level.Value - 1, 
            _stars.Value, 
            _skillLevel.Value - 1, 
            _configs.Upgrades.Get(_characterData.LevelUpgrade), 
            _configs.Upgrades.Get(_characterData.StarUpgrade), 
            _configs.Upgrades.Get(_characterData.SkillUpgrade));

        public void UpgradeLevel() => _level.Value++;

        public void UpgradeSkill(SkillSlotType slotType)
        {
            if (_skillsBySlot.TryGetValue(slotType, out var skill))
                skill.UpgradeLevel();
        }

        public void UpgradeSkill(string skillID)
        {
            if (_skillsByID.TryGetValue(skillID, out var skill))
                skill.UpgradeLevel();
        }

        public void UpgradeStars() => _stars.Value++;

        private void CalculateStats()
        {
            _upgrade = CreateUpgrade();
            _health.Value = GetHealth();
            _attack.Value = GetAttack();
        }

        public void AddShards(int amount) => _shards.Value += amount;

        public void RemoveShards(int amount) => _shards.Value -= amount;

        private float GetHealth() => _characterData.Health * _upgrade.GetStatMultiplier(StatType.Health);
        
        private float GetAttack() => _characterData.Attack * _upgrade.GetStatMultiplier(StatType.Attack);

        public int GetMaxLevel() => _configs.Stars.Get(Math.Max(0, _stars.Value - 1)).Level;

        public int GetMaxSkillLevel(SkillSlotType slotType) => _configs.Stars.Get(Math.Max(0, _stars.Value - 1)).GetMaxSkillLevel(slotType);

        public int GetSkillLevel(SkillSlotType slotType) =>
            _skillsBySlot.TryGetValue(slotType, out var skill) ? skill.Level.Value : 0;

        public CharacterSkill GetSkill(SkillSlotType slotType) =>
            _skillsBySlot.TryGetValue(slotType, out var skill) ? skill : null;

        public CharacterSkill GetSkill(string skillID) =>
            _skillsByID.TryGetValue(skillID, out var skill) ? skill : null;

        private void InitializeSkills(IReadOnlyList<SkillData> skillData)
        {
            _skills.Clear();
            _skillsByID.Clear();
            _skillsBySlot.Clear();

            foreach (var slot in SkillSlotExtensions.CharacterSlots)
            {
                var skillID = _characterData.GetSkill(slot);
                if (string.IsNullOrEmpty(skillID)) continue;

                var skill = new CharacterSkill(skillID, slot, GetInitialSkillLevel(skillID, skillData));
                _skills.Add(skill);
                _skillsByID[skill.ID] = skill;
                _skillsBySlot[skill.Slot] = skill;

                skill.Level.Skip(1).Subscribe(value =>
                {
                    _dispatcher.Dispatch(new ChangeCharacterSkillLevelEvent(_id, skill.ID, value));
                    RefreshSkillLevel();
                    CalculateStats();
                });
            }

            RefreshSkillLevel();
        }

        private int GetInitialSkillLevel(string skillID, IReadOnlyList<SkillData> skillData)
        {
            if (skillData == null) return 1;

            for (int i = 0; i < skillData.Count; i++)
            {
                if (skillData[i].ID == skillID)
                    return Math.Max(1, skillData[i].Level);
            }

            return 1;
        }

        private void RefreshSkillLevel()
        {
            var level = 1;
            for (int i = 0; i < _skills.Count; i++)
                level = Math.Max(level, _skills[i].Level.Value);

            _skillLevel.Value = level;
        }
    }
}
