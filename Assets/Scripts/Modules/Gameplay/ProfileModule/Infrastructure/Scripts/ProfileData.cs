using System;
using System.Collections.Generic;

namespace vikwhite
{
    [Serializable]
    public class ProfileData
    {
        public List<ResourceData> Resources = new();
        public string[] Squad;
    }
}