namespace vikwhite
{
    public class ChangeQuestClaimedEvent
    {
        public string EventID;
        public string QuestID;
        public bool Claimed;

        public ChangeQuestClaimedEvent(string eventId, string questId, bool claimed)
        {
            EventID = eventId;
            QuestID = questId;
            Claimed = claimed;
        }
    }
}
