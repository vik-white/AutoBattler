namespace vikwhite
{
    public class ChangeClassShardProfileHandler : ProfileHandler<ChangeClassShardEvent>
    {
        protected override void Handle(ChangeClassShardEvent evnt)
        {
            for (int i = 0; i < _profile.Data.ClassShards.Count; i++)
            {
                var data = _profile.Data.ClassShards[i];
                if (data.Class != evnt.Class || data.Rarity != evnt.Rarity) continue;
                data.Amount = evnt.Amount;
                _profile.Save();
                return;
            }
        }
    }
}
