using System;

namespace Dorado.DataExpress.Ldo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnorAttribute : Attribute
    {
    }
}