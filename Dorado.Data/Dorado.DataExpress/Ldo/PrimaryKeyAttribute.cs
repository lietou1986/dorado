using System;

namespace Dorado.DataExpress.Ldo
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
}