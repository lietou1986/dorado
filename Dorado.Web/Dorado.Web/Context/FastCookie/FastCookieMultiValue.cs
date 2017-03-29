using System;
using System.Runtime.CompilerServices;

namespace Dorado.Web.Context.FastCookie
{
    public abstract class FastCookieMultiValue
    {
        public FastCookie FastCookie { get; internal set; }

        public string PropertyName { get; internal set; }

        public void SetValue(string value, [CallerMemberName] string propertyName = "")
        {
            FastCookie.Internal_SetValues(GetType(), PropertyName, propertyName, value);
        }

        public void SetValue(int value, [CallerMemberName] string propertyName = "")
        {
            FastCookie.Internal_SetValues(GetType(), PropertyName, propertyName, value.ToString());
        }

        public void SetValue(long value, [CallerMemberName] string propertyName = "")
        {
            FastCookie.Internal_SetValues(GetType(), PropertyName, propertyName, value.ToString());
        }

        public void SetValue(bool value, [CallerMemberName] string propertyName = "")
        {
            FastCookie.Internal_SetValues(GetType(), PropertyName, propertyName, FastCookie.ConvertToString(value));
        }

        public void SetValue(double value, [CallerMemberName] string propertyName = "")
        {
            FastCookie.Internal_SetValues(GetType(), PropertyName, propertyName, FastCookie.ConvertToString(value));
        }

        public void SetValue(DateTime? value, [CallerMemberName] string propertyName = "")
        {
            FastCookie.Internal_SetValues(GetType(), PropertyName, propertyName, FastCookie.ConvertToString(value));
        }

        public string GetValue([CallerMemberName] string propertyName = "")
        {
            return FastCookie.Internal_GetValues(GetType(), PropertyName, propertyName);
        }

        public int GetValueInt([CallerMemberName] string propertyName = "")
        {
            return FastCookie.ConvertToInt(FastCookie.Internal_GetValues(GetType(), PropertyName, propertyName));
        }

        public long GetValueLong([CallerMemberName] string propertyName = "")
        {
            return FastCookie.ConvertToLong(FastCookie.Internal_GetValues(GetType(), PropertyName, propertyName));
        }

        public double GetValueDouble([CallerMemberName] string propertyName = "")
        {
            return FastCookie.ConvertToDouble(FastCookie.Internal_GetValues(GetType(), PropertyName, propertyName));
        }

        public DateTime? GetValueDateTime([CallerMemberName] string propertyName = "")
        {
            return FastCookie.ConvertToDateTime(FastCookie.Internal_GetValues(GetType(), PropertyName, propertyName));
        }

        public bool GetValueBool([CallerMemberName] string propertyName = "")
        {
            return FastCookie.ConvertToBool(FastCookie.Internal_GetValues(GetType(), PropertyName, propertyName));
        }
    }
}