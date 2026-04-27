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

        public string ID => _id;
        public IReadOnlyReactiveProperty<int> Level => _level;
        public IReadOnlyReactiveProperty<float> Health => _health;

        public Character(IConfigs configs, IEventDispatcher dispatcher)
        {
            _configs = configs;
            _dispatcher = dispatcher;
        }
        
        public void Initialize(string id, int level)
        {
            _id = id;
            _level = new ReactiveProperty<int>(level);
            _health = new ReactiveProperty<float>(GetHealth());
            _characterData = _configs.Characters.Get(id);
            _levelUpData = _configs.LevelUp.Get(_characterData.LevelUp);
        }

        public void Upgrade()
        {
            _level.Value++;
            _health.Value = GetHealth();
            _dispatcher.Dispatch(new LevelUpCharacterEvent(_id, _level.Value));
        }

        private float GetHealth() => _configs.Characters.Get(_id).Health * CharacterHandler.GetLevelMultiplier(_level.Value, _levelUpData.Health);
    }
}