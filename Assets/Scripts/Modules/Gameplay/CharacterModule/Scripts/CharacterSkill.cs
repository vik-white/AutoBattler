using UniRx;

namespace vikwhite
{
    public class CharacterSkill
    {
        private readonly ReactiveProperty<int> _level;

        public string ID { get; }
        public SkillSlotType Slot { get; }
        public IReadOnlyReactiveProperty<int> Level => _level;

        public CharacterSkill(string id, SkillSlotType slot, int level)
        {
            ID = id;
            Slot = slot;
            _level = new ReactiveProperty<int>(level);
        }

        public void UpgradeLevel() => _level.Value++;
    }
}