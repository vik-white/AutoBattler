using UniRx;
using vikwhite.Data;

namespace vikwhite
{
    public class Character
    {
        private readonly IConfigs _configs;
        private readonly IEventDispatcher _dispatcher;
        
        private ICharacterData _characterData;
        private ILevelUpData _levelUpData;
        private ILevelUpData _starLevelUpData;
        
        private string _id;
        private ReactiveProperty<float> _health;
        private ReactiveProperty<int> _level;
        private ReactiveProperty<int> _shards;
        private ReactiveProperty<int> _stars;
        private ReactiveProperty<int> _skillLevel;

        public string ID => _id;
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
            _levelUpData = _configs.LevelUp.Get(_characterData.LevelUp);
            _starLevelUpData = _configs.LevelUp.Get(_characterData.StarLevelUp);
            _level = new ReactiveProperty<int>(level);
            _shards = new ReactiveProperty<int>(shards);
            _stars = new ReactiveProperty<int>(stars);
            _skillLevel = new ReactiveProperty<int>(skillLevel);
            _health = new ReactiveProperty<float>(GetHealth());
            _level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterLevelEvent(_id, value)));
            _shards.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterShardEvent(_id, value)));
            _stars.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterStarsEvent(_id, value)));
            _skillLevel.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeCharacterSkillLevelEvent(_id, value)));
        }

        public void UpgradeLevel()
        {
            _level.Value++;
            _health.Value = GetHealth();
        }

        public void UpgradeSkill()
        {
            _skillLevel.Value++;
        }
        
        public void UpgradeStars()
        {
            _stars.Value++;
            _health.Value = GetHealth();
        }

        public void AddShards(int amount) => _shards.Value += amount;

        public void RemoveShards(int amount) => _shards.Value -= amount;

        private float GetHealth()
        {
            var multiply = CharacterHandler.GetLevelMultiplier(_level.Value, _levelUpData.Health);
            multiply *= CharacterHandler.GetLevelMultiplier(_stars.Value, _starLevelUpData.Health);
            return _configs.Characters.Get(_id).Health * multiply;
        }

        public int GetMaxLevel()
        {
            var locks = _configs.Stars.GetAll();
            for (int i = 0; i < locks.Count; i++)
            {
                if (locks[i].ID > _stars.Value) return locks[i-1].Level;
            }
            return 0;
        }
        
        public int GetMaxSkillActive()
        {
            var locks = _configs.Stars.GetAll();
            for (int i = 0; i < locks.Count; i++)
            {
                if (locks[i].ID > _stars.Value) return locks[i-1].SkillActive;
            }
            return 0;
        }
    }
}
