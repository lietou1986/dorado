using System;

namespace Dorado.ActivityEngine.ServiceImp
{
    public class TypeNotFoundException : ApplicationException
    {
        public TypeNotFoundException(string typeName)
            : base("Type '" + typeName + "' could not be found")
        {
        }
    }
}