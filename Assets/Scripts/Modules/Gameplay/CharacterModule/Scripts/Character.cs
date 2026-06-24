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
        private ReactiveProperty<int> _level;
        private ReactiveProperty<int> _shards;
        private ReactiveProperty<int> _stars;
        private readonly List<CharacterSkill> _skills = new();

        public string ID => _id;
        public ICharacterData Config => _characterData;
        public IReadOnlyReactiveProperty<int> Level => _level;
        public IReadOnlyReactiveProperty<int> Shards => _shards;
        public IReadOnlyReactiveProperty<int> Stars => _stars;
        public IReadOnlyList<CharacterSkill> Skills => _skills;
        public IUpgradeData LevelUpgrade => _configs.Upgrades.Get(_characterData.LevelUpgrade);
        public IUpgradeData StarUpgrade => _configs.Upgrades.Get(_characterData.StarUpgrade);

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
            InitializeSkills(skills);
        }

        private void InitializeSkills(IReadOnlyList<SkillData> skills)
        {
            foreach (var skillData in skills)
            {
                var slot = _characterData.GetSkillSlot(skillData.ID);
                var skill = new CharacterSkill(skillData.ID, slot, skillData.Level);
                _skills.Add(skill);
                skill.Level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterSkillLevelEvent(_id, skill.ID, value)));
            }
        }

        public void UpgradeLevel() => _level.Value++;

        public void UpgradeSkill(string id) => _skills.Find(e => e.ID == id).UpgradeLevel();
        
        public void UpgradeStars() => _stars.Value++;

        public void AddShards(int amount) => _shards.Value += amount;

        public void RemoveShards(int amount) => _shards.Value -= amount;

        public int GetMaxLevel() => _configs.Stars.Get(Math.Max(0, _stars.Value - 1)).Level;

        public int GetMaxSkillLevel(SkillSlotType slot) => _configs.Stars.Get(Math.Max(0, _stars.Value - 1)).GetMaxSkillLevel(slot);

        public int GetSkillLevel(SkillSlotType slot)
        {
            var skill = _skills.Find(e => e.Slot == slot);
            return skill != null ? skill.Level.Value : 0;
        }

        public CharacterSkill GetSkill(SkillSlotType slot) => _skills.Find(e => e.Slot == slot);
    }
}
