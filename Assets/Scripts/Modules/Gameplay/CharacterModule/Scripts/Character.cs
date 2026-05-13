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
        private ReactiveProperty<int> _level;
        private ReactiveProperty<int> _shards;
        private ReactiveProperty<int> _stars;
        private ReactiveProperty<int> _skillLevel;
        private CharacterUpgrade _upgrade;

        public string ID => _id;
        public ICharacterData Config => _characterData;
        public IReadOnlyReactiveProperty<float> Health => _health;
        public IReadOnlyReactiveProperty<int> Level => _level;
        public IReadOnlyReactiveProperty<int> Shards => _shards;
        public IReadOnlyReactiveProperty<int> Stars => _stars;
        public IReadOnlyReactiveProperty<int> SkillLevel => _skillLevel;

        public Character(IConfigs configs, IEventDispatcher dispatcher)
        {
            _configs = configs;
            _dispatcher = dispatcher;
        }
        
        public void Initialize(string id, int level, int shards, int stars, int skillLevel)
        {
            _id = id;
            _characterData = _configs.Characters.Get(id);
            _level = new ReactiveProperty<int>(level);
            _shards = new ReactiveProperty<int>(shards);
            _stars = new ReactiveProperty<int>(stars);
            _skillLevel = new ReactiveProperty<int>(skillLevel);
            _upgrade = CreateUpgrade();
            _health = new ReactiveProperty<float>(GetHealth());
            _level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterLevelEvent(_id, value)));
            _shards.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterShardEvent(_id, value)));
            _stars.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterStarsEvent(_id, value)));
            _skillLevel.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterSkillLevelEvent(_id, value)));
        }

        private CharacterUpgrade CreateUpgrade() => new (
            _level.Value - 1, 
            _stars.Value, 
            _skillLevel.Value - 1, 
            _configs.Upgrades.Get(_characterData.LevelUpgrade), 
            _configs.Upgrades.Get(_characterData.StarUpgrade), 
            _configs.Upgrades.Get(_characterData.SkillUpgrade));

        public void UpgradeLevel()
        {
            _level.Value++;
            _upgrade = CreateUpgrade();
            _health.Value = GetHealth();
        }

        public void UpgradeSkill()
        {
            _skillLevel.Value++;
            _upgrade = CreateUpgrade();
        }

        public void UpgradeStars()
        {
            _stars.Value++;
            _upgrade = CreateUpgrade();
            _health.Value = GetHealth();
        }

        public void AddShards(int amount) => _shards.Value += amount;

        public void RemoveShards(int amount) => _shards.Value -= amount;

        private float GetHealth() => _characterData.Health * _upgrade.GetStatMultiplier(StatType.Health);

        public int GetMaxLevel() => _configs.Stars.Get(_stars.Value - 1).Level;

        public int GetMaxSkillLevel(SkillSlotType slotType) => _configs.Stars.Get(_stars.Value - 1).GetSkillUnlock(slotType);
    }
}
