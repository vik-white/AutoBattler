namespace vikwhite
{
    public class ChangeCharacterLevelQuestHandler : QuestHandler<ChangeCharacterLevelEvent>
    {
        protected override void Handle(ChangeCharacterLevelEvent evnt)
        {
            var quests = _registry.Quests;
            for (int i = 0; i < quests.Count; i++)
            {
                var quest = quests[i];
                if (quest == null || quest.Claimed.Value) continue;
                if (quest.TargetID != evnt.ID) continue;

                switch (quest.Type)
                {
                    case QuestType.CharacterLevelUpAmount:
                        if (quest.Progress.Value < quest.Amount) quest.Progress.Value += 1;
                        break;

                    case QuestType.CharacterReachLevel:
                        if (evnt.Level > quest.Progress.Value) quest.Progress.Value = evnt.Level;
                        break;
                }
            }
        }
    }
}
