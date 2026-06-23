using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace vikwhite
{
    public class SkillItemViewModel : ViewModel<SkillItemModel>
    {
        private readonly ReactiveProperty<bool> _isVisible = new();
        private readonly ReactiveProperty<bool> _isSelected = new();
        private readonly ReactiveProperty<bool> _isLocked = new();
        private readonly ReactiveProperty<int> _level = new();

        public SkillSlotType Slot => Model.Slot;
        public string Name => Model.Skill.ID;
        public string Description => Model.Skill?.Description ?? "";
        public Sprite Icon => Model.Skill?.IconImage;
        public IReadOnlyReactiveProperty<bool> IsVisible => _isVisible;
        public IReadOnlyReactiveProperty<bool> IsSelected => _isSelected;
        public IReadOnlyReactiveProperty<bool> IsLocked => _isLocked;
        public IReadOnlyReactiveProperty<int> Level => _level;
        public UnityAction OnSelect;

        public SkillItemViewModel(SkillItemModel model) : base(model)
        {
            AddDisposables(_isVisible, _isSelected, _isLocked, _level);
            _isVisible.Value = model.HasSkill;
        }

        public void SetSelected(bool selected)
        {
            _isSelected.Value = selected;
        }

        public void SetLevel(int level)
        {
            _level.Value = level;
            _isLocked.Value = level <= 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}
