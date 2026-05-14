namespace vikwhite
{
    public class ChangeClassBookProfileHandler : ProfileHandler<ChangeClassBookEvent>
    {
        protected override void Handle(ChangeClassBookEvent evnt)
        {
            for (int i = 0; i < _profile.Data.ClassBooks.Count; i++)
            {
                var data = _profile.Data.ClassBooks[i];
                if (data.Class != evnt.Class) continue;
                data.Amount = evnt.Amount;
                _profile.Save();
                return;
            }
        }
    }
}
