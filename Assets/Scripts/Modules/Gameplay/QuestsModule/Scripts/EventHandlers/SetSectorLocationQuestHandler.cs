namespace vikwhite
{
    public class SetSectorLocationQuestHandler : QuestHandler<SetSectorLocationEvent>
    {
        protected override void Handle(SetSectorLocationEvent evnt)
        {
            var completedLocationID = evnt.PreviousID;
            if (string.IsNullOrEmpty(completedLocationID)) return;

            var quests = _registry.Quests;
            for (int i = 0; i < quests.Count; i++)
            {
                var quest = quests[i];
                if (quest == null || quest.Claimed.Value) continue;

                switch (quest.Type)
                {
                    case QuestType.CompleteLevel:
                        if (quest.TargetID != completedLocationID) continue;
                        if (quest.Progress.Value < quest.Amount) quest.Progress.Value = quest.Amount;
                        break;

                    case QuestType.CompleteLevels:
                        if (quest.Progress.Value < quest.Amount) quest.Progress.Value += 1;
                        break;
                }
            }
        }
    }
}
