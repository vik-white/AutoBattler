namespace vikwhite
{
    public class ChangeQuestProgressProfileHandler : ProfileHandler<ChangeQuestProgressEvent>
    {
        protected override void Handle(ChangeQuestProgressEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Quests.Count; i++)
            {
                if (_profile.Data.Quests[i].EventID != evnt.EventID) continue;
                if (_profile.Data.Quests[i].QuestID != evnt.QuestID) continue;
                _profile.Data.Quests[i].Progress = evnt.Progress;
                _profile.Save();
                return;
            }
        }
    }
}
