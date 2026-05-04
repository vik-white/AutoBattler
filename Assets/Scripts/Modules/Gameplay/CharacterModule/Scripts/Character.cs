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
        private string _id;
        private ReactiveProperty<int> _level;
        private ReactiveProperty<float> _health;
        private ReactiveProperty<int> _shards;

        public string ID => _id;
        public IReadOnlyReactiveProperty<int> Level => _level;
        public IReadOnlyReactiveProperty<float> Health => _health;
        public IReadOnlyReactiveProperty<int> Shards => _shards;

        public Character(IConfigs configs, IEventDispatcher dispatcher)
        {
            _configs = configs;
            _dispatcher = dispatcher;
        }
        
        public void Initialize(string id, int level, int shards)
        {
            _id = id;
            _characterData = _configs.Characters.Get(id);
            _levelUpData = _configs.LevelUp.Get(_characterData.LevelUp);
            _level = new ReactiveProperty<int>(level);
            _health = new ReactiveProperty<float>(GetHealth());
            _shards = new ReactiveProperty<int>(shards);
            _shards.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeShardEvent(_id, value)));
            _level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new LevelUpCharacterEvent(_id, value)));
        }

        public void Upgrade()
        {
            _level.Value++;
            _health.Value = GetHealth();
        }

        public void AddShards(int amount)
        {
            if (amount <= 0) return;
            _shards.Value += amount;
        }

        private float GetHealth() => _configs.Characters.Get(_id).Health * CharacterHandler.GetLevelMultiplier(_level.Value, _levelUpData.Health);
    }
}
