using System;

namespace Dorado.Core.ComponentModel
{ 
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)] 
    public sealed class ObjectSignatureAttribute : Attribute
    {
    }
}
