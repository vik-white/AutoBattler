namespace vikwhite
{
    public class ChangeQuestProgressEvent
    {
        public string EventID;
        public string QuestID;
        public int Progress;

        public ChangeQuestProgressEvent(string eventId, string questId, int progress)
        {
            EventID = eventId;
            QuestID = questId;
            Progress = progress;
        }
    }
}
