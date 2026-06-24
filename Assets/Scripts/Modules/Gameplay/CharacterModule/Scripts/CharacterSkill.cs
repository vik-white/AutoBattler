using UniRx;

namespace vikwhite
{
    public class CharacterSkill
    {
        private readonly ReactiveProperty<int> _level;
        private Data.ISkillData _config; 
        
        public Data.ISkillData Config => _config;
        public string ID => _config.ID;
        public SkillSlotType Slot { get; }
        public IReadOnlyReactiveProperty<int> Level => _level;

        public CharacterSkill(Data.ISkillData config, SkillSlotType slot, int level)
        {
            _config = config;
            Slot = slot;
            _level = new ReactiveProperty<int>(level);
        }

        public void UpgradeLevel() => _level.Value++;
    }
}