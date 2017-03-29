using System;

namespace Dorado.Web.Context.FastRequestItem
{
    /// <summary>
    /// Indicates a property that should not be saved to a request item
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FastRequestItemSkipAttribute : Attribute
    {
    }

    /// <summary>
    /// Define how this Property will be converted to a request item
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FastRequestItemSettingsAttribute : Attribute
    {
        public FastRequestItemSettingsAttribute()
        { }

        public FastRequestItemSettingsAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}