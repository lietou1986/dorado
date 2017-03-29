using System;
using System.Collections.Generic;

namespace Dorado.Web.Fileset
{
    public class StaticFileDictionary : Dictionary<string, StaticFile>
    {
        public StaticFileDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public StaticFileDictionary(int capacity)
            : base(capacity, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}