using System.Collections.Generic;

namespace vikwhite.Data
{
    public interface ICustomJsonParser
    {
        void Parse(Dictionary<string, string> row);
    }
}