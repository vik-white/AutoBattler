namespace vikwhite
{
    public class AddShardProfileHandler : ProfileHandler<AddShardEvent>
    {
        protected override void Handle(AddShardEvent evnt)
        {
            ShardData entry = null;
            for (int i = 0; i < _profile.Data.Shards.Count; i++)
            {
                if (_profile.Data.Shards[i].ID != evnt.ID) continue;
                entry = _profile.Data.Shards[i];
                break;
            }

            if (entry != null)
                entry.Count += evnt.Amount;
            else
                _profile.Data.Shards.Add(new ShardData { ID = evnt.ID, Count = evnt.Amount });

            _profile.Save();
        }
    }
}
