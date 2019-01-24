using System;

namespace Dorado.Platform.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DisplayOrderAttribute : Attribute
    {
        public DisplayOrderAttribute(int displayOrder)
        {
            DisplayOrder = displayOrder;
        }

        public int DisplayOrder { get; set; }
    }
}