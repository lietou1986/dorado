using System;
using System.Collections.Generic;

namespace Dorado.Web.Fileset
{
    public class VariableDictionary : Dictionary<string, string>
    {
        public VariableDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public VariableDictionary(int capacity)
            : base(capacity, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}