using System.Collections.Generic;

namespace vikwhite
{
    public class EventWindowViewModel : WindowViewModel<GameEvent>
    {
        public string Title;
        public GameEventType Type;
        public List<QuestItemViewModel> Quests = new();

        public EventWindowViewModel(GameEvent model) : base(model)
        {
            Title = model.Name;
            Type = model.Type;

            if (model.Type != GameEventType.Quest) return;

            foreach (var quest in model.Quests)
                Quests.Add(CreateViewModel<QuestItemViewModel, Quest>(quest));
        }
    }
}
