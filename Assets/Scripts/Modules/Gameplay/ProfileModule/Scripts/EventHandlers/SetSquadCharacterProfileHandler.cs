using UnityEngine;

namespace vikwhite
{
    public class SetSquadCharacterProfileHandler : ProfileHandler<SetSquadCharacterEvent>
    {
        protected override void Handle(SetSquadCharacterEvent evnt)
        {
            _profile.Data.Squad[evnt.Index] = evnt.ID;
            _profile.Save();
        }
    }
}