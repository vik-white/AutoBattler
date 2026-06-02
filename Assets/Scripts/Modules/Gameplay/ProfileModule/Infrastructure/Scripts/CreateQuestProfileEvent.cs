namespace vikwhite
{
    public class CreateQuestProfileEvent
    {
        public string EventID;
        public string QuestID;

        public CreateQuestProfileEvent(string eventId, string questId)
        {
            EventID = eventId;
            QuestID = questId;
        }
    }
}
