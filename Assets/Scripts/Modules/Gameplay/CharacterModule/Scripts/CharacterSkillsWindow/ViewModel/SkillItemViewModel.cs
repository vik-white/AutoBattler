using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SkillItemViewModel : ViewModel<CharacterSkill>
    {
        private readonly ISkillData _config;
        private readonly ReactiveProperty<bool> _isVisible = new();
        private readonly ReactiveProperty<bool> _isSelected = new();
        private readonly ReactiveProperty<bool> _isLocked = new();

        public SkillSlotType Slot => Model.Slot;
        public string ID => Model.ID;
        public string Name => _config.Name;
        public string Description => _config?.Description ?? "";
        public Sprite Icon => _config?.IconImage;
        public IReadOnlyReactiveProperty<bool> IsVisible => _isVisible;
        public IReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        public IReadOnlyReactiveProperty<bool> IsLocked => _isLocked;
        public IReadOnlyReactiveProperty<int> Level => Model.Level;
        public UnityAction OnSelect;

        public SkillItemViewModel(CharacterSkill model, IConfigs configs) : base(model)
        {
            _config = configs.Skills.Get(model.ID);
            AddDisposables(_isVisible, _isSelected, _isLocked);
            _isVisible.Value = _config != null;
        }

        public void SetSelected(bool selected)
        {
            _isSelected.Value = selected;
        }

        public void SetLevel(int level)
        {
            _isLocked.Value = level <= 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}
