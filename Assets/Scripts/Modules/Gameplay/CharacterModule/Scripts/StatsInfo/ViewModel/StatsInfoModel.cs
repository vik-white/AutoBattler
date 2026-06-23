using UniRx;

namespace vikwhite
{
    public class StatsInfoModel
    {
        public Character Character { get; }
        public IReadOnlyReactiveProperty<int> CurrentLevel { get; }
        public IReadOnlyReactiveProperty<int> CurrentStars { get; }
        public IReadOnlyReactiveProperty<int> SelectedLevel { get; }
        public IReadOnlyReactiveProperty<int> SelectedStars { get; }

        public StatsInfoModel(
            Character character,
            IReadOnlyReactiveProperty<int> currentLevel,
            IReadOnlyReactiveProperty<int> currentStars,
            IReadOnlyReactiveProperty<int> selectedLevel,
            IReadOnlyReactiveProperty<int> selectedStars)
        {
            Character = character;
            CurrentLevel = currentLevel;
            CurrentStars = currentStars;
            SelectedLevel = selectedLevel;
            SelectedStars = selectedStars;
        }
    }
}
