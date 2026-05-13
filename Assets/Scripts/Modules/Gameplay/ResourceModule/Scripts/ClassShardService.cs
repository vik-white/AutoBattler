using System.Collections.Generic;
using UniRx;

namespace vikwhite
{
    public interface IClassShardService
    {
        void Initialize();
        ClassShard Get(CharacterClassType @class, RarityType rarity);
        ClassShard Get(ClassShardKey key);
        ReactiveProperty<int> GetAmount(CharacterClassType @class, RarityType rarity);
        ReactiveProperty<int> GetAmount(ClassShardKey key);
        bool CanSpend(CharacterClassType @class, RarityType rarity, int amount);
        bool CanSpend(ClassShardKey key, int amount);
        void Add(CharacterClassType @class, RarityType rarity, int amount);
        void Add(ClassShardKey key, int amount);
        void Spend(CharacterClassType @class, RarityType rarity, int amount);
        void Spend(ClassShardKey key, int amount);
        IReadOnlyCollection<ClassShard> GetAll();
    }

    public class ClassShardService : IClassShardService
    {
        private readonly IProfileService _profile;
        private readonly IEventDispatcher _dispatcher;
        private readonly Dictionary<ClassShardKey, ClassShard> _shards = new();

        public ClassShardService(IProfileService profile, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _shards.Clear();
            foreach (var data in _profile.Data.ClassShards)
            {
                var key = new ClassShardKey(data.Class, data.Rarity);
                _shards[key] = new ClassShard(key, data.Amount);
            }
        }

        public ClassShard Get(CharacterClassType @class, RarityType rarity) => Get(new ClassShardKey(@class, rarity));
        public ClassShard Get(ClassShardKey key) => _shards.TryGetValue(key, out var shard) ? shard : null;

        public ReactiveProperty<int> GetAmount(CharacterClassType @class, RarityType rarity) => GetAmount(new ClassShardKey(@class, rarity));
        public ReactiveProperty<int> GetAmount(ClassShardKey key) => Get(key)?.Amount;

        public bool CanSpend(CharacterClassType @class, RarityType rarity, int amount) => CanSpend(new ClassShardKey(@class, rarity), amount);
        public bool CanSpend(ClassShardKey key, int amount) => amount > 0 && Get(key) is { } shard && shard.Amount.Value >= amount;

        public void Add(CharacterClassType @class, RarityType rarity, int amount) => Add(new ClassShardKey(@class, rarity), amount);

        public void Add(ClassShardKey key, int amount)
        {
            if (amount <= 0) return;
            var shard = Get(key);
            if (shard == null) return;
            shard.Amount.Value += amount;
            _dispatcher.Dispatch(new ChangeClassShardEvent(key.Class, key.Rarity, shard.Amount.Value));
        }

        public void Spend(CharacterClassType @class, RarityType rarity, int amount) => Spend(new ClassShardKey(@class, rarity), amount);

        public void Spend(ClassShardKey key, int amount)
        {
            if (amount <= 0) return;
            var shard = Get(key);
            if (shard == null) return;
            if (shard.Amount.Value - amount < 0) return;
            shard.Amount.Value -= amount;
            _dispatcher.Dispatch(new ChangeClassShardEvent(key.Class, key.Rarity, shard.Amount.Value));
        }

        public IReadOnlyCollection<ClassShard> GetAll() => _shards.Values;
    }
}
