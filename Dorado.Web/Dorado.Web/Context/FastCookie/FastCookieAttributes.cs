using System;

namespace Dorado.Web.Context.FastCookie
{
    /// <summary>
    /// Indicates a property that should not be saved to a cookie
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FastCookieSkipAttribute : Attribute
    {
    }

    /// <summary>
    /// Define how this Property will be converted to a cookie
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FastCookieSettingsAttribute : Attribute
    {
        public FastCookieSettingsAttribute()
        { }

        public FastCookieSettingsAttribute(string name)
        {
            Name = name;
        }

        public FastCookieSettingsAttribute(string name, int expiresInDays)
        {
            Name = name;
            ExpiresInDays = expiresInDays;
        }

        public string Name { get; set; }

        public int ExpiresInDays { get; set; }

        public bool HttpOnly { get; set; }

        public string Domain { get; set; }

        public string Path { get; set; }

        public bool Secure { get; set; }

        public int MaxLength { get; set; }
    }
}