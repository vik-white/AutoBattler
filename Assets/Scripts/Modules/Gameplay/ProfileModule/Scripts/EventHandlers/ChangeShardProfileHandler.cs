namespace vikwhite
{
    public class ChangeShardProfileHandler : ProfileHandler<ChangeShardEvent>
    {
        protected override void Handle(ChangeShardEvent evnt)
        {
            for (int i = 0; i < _profile.Data.Characters.Count; i++)
            {
                if (_profile.Data.Characters[i].ID != evnt.ID) continue;
                _profile.Data.Characters[i].Shards = evnt.Count;
                _profile.Save();
                return;
            }
        }
    }
}
