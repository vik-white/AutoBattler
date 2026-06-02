namespace vikwhite
{
    public class CreateQuestProfileHandler : ProfileHandler<CreateQuestProfileEvent>
    {
        protected override void Handle(CreateQuestProfileEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Quests.Count; i++)
            {
                if (_profile.Data.Quests[i].EventID != evnt.EventID) continue;
                if (_profile.Data.Quests[i].QuestID != evnt.QuestID) continue;
                return;
            }

            _profile.Data.Quests.Add(new QuestProfileData
            {
                EventID = evnt.EventID,
                QuestID = evnt.QuestID,
                Progress = 0,
                Claimed = false
            });
            _profile.Save();
        }
    }
}
