namespace vikwhite
{
    public class ChangeResourceQuestHandler : QuestHandler<ChangeResourceEvent>
    {
        protected override void Handle(ChangeResourceEvent evnt)
        {
            if (evnt.Delta == 0) return;

            var targetType = evnt.Delta > 0 ? QuestType.CollectResource : QuestType.SpendResource;
            var delta = evnt.Delta > 0 ? evnt.Delta : -evnt.Delta;

            var quests = _registry.Quests;
            for (int i = 0; i < quests.Count; i++)
            {
                var quest = quests[i];
                if (quest == null || quest.Claimed.Value) continue;
                if (quest.Type != targetType) continue;
                if (quest.Resource != evnt.Type) continue;
                if (quest.Progress.Value >= quest.Amount) continue;

                var next = quest.Progress.Value + delta;
                if (next > quest.Amount) next = quest.Amount;
                quest.Progress.Value = next;
            }
        }
    }
}
