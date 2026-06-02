using System.Collections.Generic;

namespace vikwhite
{
    public class EventWindowViewModel : WindowViewModel<GameEvent>
    {
        public string Title;
        public GameEventType Type;
        public List<QuestItemViewModel> Quests = new();

        public EventWindowViewModel(GameEvent model, IQuestsService questsService) : base(model)
        {
            Title = model.Name;
            Type = model.Type;

            if (model.Type == GameEventType.Quest)
            {
                foreach (var questId in model.QuestIds)
                {
                    var quest = questsService.Get(questId);
                    if (quest == null) continue;
                    Quests.Add(CreateViewModel<QuestItemViewModel, Quest>(quest));
                }
            }
        }
    }
}
