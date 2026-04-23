using UnityEngine;

namespace vikwhite
{
    public class ChangeResourceProfileHandler : ProfileHandler<ChangeResourceEvent>
    {
        protected override void Handle(ChangeResourceEvent evnt)
        {
            Debug.Log("ChangeResourceProfileHandler");
            foreach (var resourceData in _profile.Data.Resources)
            {
                if (resourceData.Type == evnt.Type)
                    resourceData.Amount = evnt.Amount;
            }
            _profile.Save();
        }
    }
}