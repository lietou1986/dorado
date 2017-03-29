using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;

namespace Dorado.Web.Context.FastRequestItem
{
    public abstract class FastRequestItem
    {
        /// <summary>
        /// HttpContextBase of the Current Request & Response
        /// </summary>
        public HttpContextBase Context { get; private set; }

        private static ConcurrentDictionary<Type, Dictionary<string, FastRequestItemSettingsAttribute>> _RequestItemSettings;

        static FastRequestItem()
        {
            _RequestItemSettings = new ConcurrentDictionary<Type, Dictionary<string, FastRequestItemSettingsAttribute>>();
        }

        protected FastRequestItem(HttpContextBase context)
        {
            Context = context;
        }

        // ====================================================================
        //
        // GET Value
        //
        // ====================================================================
        protected string GetValue([CallerMemberName] string propertyName = "")
        {
            return Internal_GetValue(propertyName);
        }

        protected int GetValueInt([CallerMemberName] string propertyName = "")
        {
            return ConvertToInt(Internal_GetValue(propertyName));
        }

        protected long GetValueLong([CallerMemberName] string propertyName = "")
        {
            return ConvertToLong(Internal_GetValue(propertyName));
        }

        protected double GetValueDouble([CallerMemberName] string propertyName = "")
        {
            return ConvertToDouble(Internal_GetValue(propertyName));
        }

        protected bool GetValueBool([CallerMemberName] string propertyName = "")
        {
            return ConvertToBool(Internal_GetValue(propertyName));
        }

        protected DateTime? GetValueDateTime([CallerMemberName] string propertyName = "")
        {
            return ConvertToDateTime(Internal_GetValue(propertyName));
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = "") where T : new()
        {
            return (T)Internal_GetRequestItem(propertyName);
        }

        // ====================================================================
        //
        // SET Value
        //
        // ====================================================================
        public void ClearRequestItem(string sessionName)
        {
            Internal_RemoveRequestItem(sessionName);
        }

        protected bool SetValue(string value, [CallerMemberName] string propertyName = "")
        {
            return Internal_SetValue(propertyName, value);
        }

        protected bool SetValue(int value, [CallerMemberName] string propertyName = "")
        {
            if (value == 0)
            {
                Internal_RemoveValue(propertyName);
                return true;
            }
            return Internal_SetValue(propertyName, value.ToString());
        }

        protected bool SetValue(long value, [CallerMemberName] string propertyName = "")
        {
            if (value == 0)
            {
                Internal_RemoveValue(propertyName);
                return true;
            }
            return Internal_SetValue(propertyName, value.ToString());
        }

        protected bool SetValue(double value, [CallerMemberName] string propertyName = "")
        {
            if (double.IsNaN(value) || value == 0)
            {
                Internal_RemoveValue(propertyName);
                return true;
            }

            return Internal_SetValue(propertyName, ConvertToString(value));
        }

        protected bool SetValue(bool value, [CallerMemberName] string propertyName = "")
        {
            return Internal_SetValue(propertyName, ConvertToString(value));
        }

        protected bool SetValue(DateTime? value, [CallerMemberName] string propertyName = "")
        {
            if (!value.HasValue || value == DateTime.MinValue)
            {
                Internal_RemoveValue(propertyName);
                return true;
            }
            return Internal_SetValue(propertyName, ConvertToString(value));
        }

        protected bool SetValue<T>(T value, [CallerMemberName] string propertyName = "") where T : new()
        {
            Internal_SetRequestItem(propertyName, value);
            return true;
        }

        // ====================================================================
        //
        // RequestItem Settings
        //
        // ====================================================================
        static private Dictionary<string, FastRequestItemSettingsAttribute> ProcessTypeSettings(Type type)
        {
            var typeSettings = new Dictionary<string, FastRequestItemSettingsAttribute>();

            var usedNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var settings =
                    property.GetCustomAttribute(typeof(FastRequestItemSettingsAttribute)) as
                        FastRequestItemSettingsAttribute;
                if (settings == null)
                    continue;

                typeSettings[property.Name] = settings;

                if (!string.IsNullOrWhiteSpace(settings.Name))
                {
                    if (usedNames.Contains(settings.Name))
                    {
                        Trace.WriteLine("Type " + type + " used the same name twice: " + settings.Name);
                    }
                    usedNames.Add(settings.Name);
                }
            }
            _RequestItemSettings[type] = typeSettings;

            return typeSettings;
        }

        private FastRequestItemSettingsAttribute GetPropertySettings(string propertyName, Type type = null)
        {
            if (type == null)
            {
                type = GetType();
            }

            Dictionary<string, FastRequestItemSettingsAttribute> typeSettings;
            if (!_RequestItemSettings.TryGetValue(type, out typeSettings))
            {
                // Means we haven't seen this type yet, enumerate it
                typeSettings = ProcessTypeSettings(type);
            }

            FastRequestItemSettingsAttribute settings;
            typeSettings.TryGetValue(propertyName, out settings);
            return settings;
        }

        private string ConvertPropertyNameToRequestItemName(string propertyName)
        {
            string sessionName = propertyName;
            FastRequestItemSettingsAttribute setting = GetPropertySettings(propertyName);
            if (setting != null)
            {
                sessionName = setting.Name;
            }
            return sessionName;
        }

        // ====================================================================
        //
        // Internal Set & Get Single Value
        //
        // ====================================================================
        private string Internal_GetValue(string propertyName)
        {
            var session = Internal_GetRequestItem(propertyName);

            return session.ToString();
        }

        private bool Internal_SetValue(string propertyName, string value)
        {
            var oldValue = Internal_GetValue(propertyName);
            if (oldValue == value)
                return false;

            if (string.IsNullOrWhiteSpace(value))
            {
                Internal_RemoveValue(propertyName);
                return true;
            }

            Internal_SetRequestItem(propertyName, value);

            return true;
        }

        private void Internal_RemoveValue(string propertyName)
        {
            var sessionName = ConvertPropertyNameToRequestItemName(propertyName);
            Internal_RemoveRequestItem(sessionName);
        }

        private void Internal_RemoveRequestItem(string sessionName)
        {
            if (string.IsNullOrWhiteSpace(sessionName))
                return;

            if (Context == null || Context.Items == null)
                return;

            Context.Items.Remove(sessionName);
        }

        private object Internal_GetRequestItem(string propertyName)
        {
            if (Context == null || Context.Items == null)
                return null;

            var sessionName = ConvertPropertyNameToRequestItemName(propertyName);

            return Context.Items[sessionName];
        }

        private void Internal_SetRequestItem(string sessionName, object value)
        {
            if (Context == null || Context.Items == null)
                return;
            Context.Items[sessionName] = value;
        }

        // ====================================================================
        //
        // Standard Conversion
        //
        // ====================================================================
        static internal string ConvertToString(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return dateTime.Value.ToString("s");
        }

        static internal string ConvertToString(bool boolean)
        {
            return boolean ? "1" : "0";
        }

        static internal string ConvertToString(double dvalue)
        {
            return dvalue.ToString("R");
        }

        static internal int ConvertToInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            int ret;
            int.TryParse(value, out ret);
            return ret;
        }

        static internal long ConvertToLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            long ret;
            long.TryParse(value, out ret);
            return ret;
        }

        static internal double ConvertToDouble(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            double ret;
            double.TryParse(value, out ret);
            return ret;
        }

        static internal DateTime? ConvertToDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            try
            {
                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                    return dt;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static internal bool ConvertToBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            switch (value)
            {
                case "1":
                case "true":
                case "True":
                case "TRUE":
                case "yes":
                    return true;
            }
            return false;
        }
    }
}