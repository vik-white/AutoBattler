namespace vikwhite
{
    public class ChangeQuestClaimedProfileHandler : ProfileHandler<ChangeQuestClaimedEvent>
    {
        protected override void Handle(ChangeQuestClaimedEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Quests.Count; i++)
            {
                if (_profile.Data.Quests[i].EventID != evnt.EventID) continue;
                if (_profile.Data.Quests[i].QuestID != evnt.QuestID) continue;
                _profile.Data.Quests[i].Claimed = evnt.Claimed;
                _profile.Save();
                return;
            }
        }
    }
}
