using System;

namespace Dorado.Web.Context.FastSession
{
    /// <summary>
    /// Indicates a property that should not be saved to a session
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FastSessionSkipAttribute : Attribute
    {
    }

    /// <summary>
    /// Define how this Property will be converted to a session
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FastSessionSettingsAttribute : Attribute
    {
        public FastSessionSettingsAttribute()
        { }

        public FastSessionSettingsAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}