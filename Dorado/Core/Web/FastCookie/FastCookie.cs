using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;

namespace Dorado.Core.Web.FastCookie
{

	public abstract class FastCookie
	{
		/// <summary>
		/// HttpContextBase of the Current Request & Response
		/// </summary>
		public HttpContextBase Context { get; private set; }

		/// <summary>
		/// Default domain to be applied to the cookies (default is empty, which means the Hostname of the URL Request). 
		/// It can be overriden with FastCookieSettings attribute by setting the Domain property.
		/// </summary>
		public string DefaultDomain { get; set; }
		/// <summary>
		/// Default Expire value for the cookie. 
		/// It can be overriden with FastCookieSettings attribute by setting the ExpiresInDays value.
		/// </summary>
		public TimeSpan? DefaultExpires { get; set; }
		/// <summary>
		/// Default Secure connection for all cookies, meaning the cookie will only be sent back to the browser if the request is HTTPS
		/// It can be overriden with FastCookieSettings attribute by setting the Secure value.
		/// </summary>
		public bool? DefaultSecure { get; set; }


		/// <summary>
		/// Maximum value length allowed for a whole cookie (default is 2048)
		/// </summary>
		static public int AutoTruncateCookieValueLength { get; set; }
		/// <summary>
		/// Maximum value length allowed for a multi-value cookie (default is 512)
		/// </summary>
		static public int AutoTruncateCookieMultiValueLength { get; set; }


		private List<FastCookieMultiValue> _MultiValuesCookies;

		private static ConcurrentDictionary<Type, Dictionary<string, FastCookieSettingsAttribute>> _CookieSettings;

		static FastCookie()
		{
			_CookieSettings = new ConcurrentDictionary<Type, Dictionary<string, FastCookieSettingsAttribute>>();
			AutoTruncateCookieValueLength = 2048;
			AutoTruncateCookieMultiValueLength = 512;
		}

	    protected FastCookie(HttpContextBase context)
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

		protected T GetMultiValue<T>([CallerMemberName] string propertyName = "") where T : FastCookieMultiValue, new()
		{
			if (_MultiValuesCookies != null)
			{
				foreach (var cmv in _MultiValuesCookies)
				{
					if (cmv.PropertyName == propertyName)
					{
						return cmv as T;
					}
				}
			}
			else
			{
				_MultiValuesCookies = new List<FastCookieMultiValue>();
			}

			var multiValueCookie = new T { FastCookie = this, PropertyName = propertyName };
			_MultiValuesCookies.Add(multiValueCookie);

			return multiValueCookie;
		}




		// ====================================================================
		//
		// SET Value
		//
		// ====================================================================
		public void ClearCookie(string cookieName)
		{
			Internal_RemoveCookie(cookieName);
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


		// ====================================================================
		//
		// Cookie Settings
		//
		// ====================================================================
		static private Dictionary<string, FastCookieSettingsAttribute> ProcessTypeSettings(Type type)
		{
			bool isFastCookieMultiValue = type.IsSubclassOf(typeof(FastCookieMultiValue));
			var typeSettings = new Dictionary<string, FastCookieSettingsAttribute>();


			var usedNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				var settings =
					property.GetCustomAttribute(typeof(FastCookieSettingsAttribute)) as
						FastCookieSettingsAttribute;
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

				if (isFastCookieMultiValue)
				{
					// Multi value cookies cannot have cookie-level properties
					if (settings.ExpiresInDays > 0 || settings.Secure || settings.Domain != null || settings.HttpOnly ||
						settings.Path != null)
					{
						Trace.WriteLine("Type " + type + " define FastCookieSettings elements that will be ignored.");
					}
				}
			}
			_CookieSettings[type] = typeSettings;

			return typeSettings;
		}


		private FastCookieSettingsAttribute GetPropertySettings(string propertyName, Type type = null)
		{
			if (type == null)
			{
				type = GetType();
			}

			Dictionary<string, FastCookieSettingsAttribute> typeSettings;
			if (!_CookieSettings.TryGetValue(type, out typeSettings))
			{
				// Means we haven't seen this type yet, enumerate it
				typeSettings = ProcessTypeSettings(type);
			}

			FastCookieSettingsAttribute settings;
			typeSettings.TryGetValue(propertyName, out settings);
			return settings;
		}

		private string ConvertPropertyNameToCookieName(string propertyName)
		{
			string cookieName = propertyName;
			FastCookieSettingsAttribute setting = GetPropertySettings(propertyName);
			if (setting != null)
			{
				cookieName = setting.Name;
			}
			return cookieName;
		}

		private Tuple<string, int> ConvertValuePropertyNameToCookieValueName(Type multiValueType, string valuePropertyName)
		{
			string cookieValueName = valuePropertyName;
			FastCookieSettingsAttribute settingSubvalue = GetPropertySettings(valuePropertyName, multiValueType);

			int autoTruncateLength = AutoTruncateCookieMultiValueLength;
			if (settingSubvalue != null)
			{
				if (settingSubvalue.MaxLength != 0)
					autoTruncateLength = settingSubvalue.MaxLength;

				if (!string.IsNullOrWhiteSpace(settingSubvalue.Name))
					cookieValueName = settingSubvalue.Name;
			}


			return Tuple.Create(cookieValueName, autoTruncateLength);
		}


		// ====================================================================
		//
		// Internal Set & Get Single Value
		//
		// ====================================================================
		private string Internal_GetValue(string propertyName)
		{
			var cookie = Internal_GetCookie(propertyName);
			if (cookie == null)
				return null;

			return cookie.Value;
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

			int truncateLength = AutoTruncateCookieValueLength;
			var settings = GetPropertySettings(propertyName);
			if (settings != null && settings.MaxLength > 0)
			{
				truncateLength = settings.MaxLength;
			}

			var newCookie = CreateRawCookie(propertyName);
			if (value.Length > truncateLength)
			{
				value = value.Substring(0, truncateLength);
			}
			newCookie.Value = value;

			Internal_SetResponseCookie(newCookie);

			return true;
		}

		private void Internal_RemoveValue(string propertyName)
		{
			var cookieName = ConvertPropertyNameToCookieName(propertyName);
			Internal_RemoveCookie(cookieName);
		}

		private void Internal_RemoveCookie(string cookieName)
		{
			if (string.IsNullOrWhiteSpace(cookieName))
				return;

			var deleteCookie = new HttpCookie(cookieName, "")
			{
				Expires = DateTime.UtcNow.AddSeconds(-1)
			};

			Internal_SetResponseCookie(deleteCookie);
		}


		// ====================================================================
		//
		// Internal Set & Get Multi Values
		//
		// ====================================================================
		private NameValueCollection Internal_GetValues(string propertyName)
		{
			var cookie = Internal_GetCookie(propertyName);
			if (cookie == null)
				return null;
			return cookie.Values;
		}

		internal string Internal_GetValues(Type multiValueType, string cookiePropertyName, string valuePropertyName)
		{
			var allValues = Internal_GetValues(cookiePropertyName);
			if (allValues == null || allValues.Count == 0)
				return null;

			var mvSetting = ConvertValuePropertyNameToCookieValueName(multiValueType, valuePropertyName);
			string cookieValueName = mvSetting.Item1;

			return allValues.Get(cookieValueName);
		}


		internal bool Internal_SetValues(Type multiValueType, string cookiePropertyName, string valuePropertyName, string value)
		{
			if (Context == null)
				return false;

			var oldValue = Internal_GetValues(multiValueType, cookiePropertyName, valuePropertyName);
			if (oldValue == value)
				return false; // nothing to set

			string cookieName = ConvertPropertyNameToCookieName(cookiePropertyName);
			var mvSetting = ConvertValuePropertyNameToCookieValueName(multiValueType, valuePropertyName);

			// Try the response first
			var cookie = Harmless_Get(Context.Response.Cookies, cookieName);
			if (cookie == null)
			{
				cookie = CreateRawCookie(cookiePropertyName);

				// See if exists in the request, if so copy existing values
				var reqCookie = Harmless_Get(Context.Request.Cookies, cookieName);
				if (reqCookie != null)
				{
					foreach (var reqValueKey in reqCookie.Values.AllKeys)
					{
						cookie.Values[reqValueKey] = reqCookie[reqValueKey];
					}
				}
				Internal_SetResponseCookie(cookie);
			}

			string cookieValueName = mvSetting.Item1;
			if (value != null)
			{
				int autoTruncateLength = mvSetting.Item2;
				if (value.Length > autoTruncateLength)
				{
					value = value.Substring(0, autoTruncateLength);
				}
			}

			cookie[cookieValueName] = value;

			return true;
		}

		// ====================================================================
		//
		// HttpCookie Helpers
		//
		// ====================================================================
		private HttpCookie Harmless_Get(HttpCookieCollection cookies, string cookieName)
		{
			if (cookies == null || cookies.Count == 0)
				return null;

			if (!cookies.AllKeys.Contains(cookieName))
				return null;

			return cookies[cookieName];
		}

		private HttpCookie Internal_GetCookie(string propertyName)
		{
			if (Context == null)
				return null;

			var cookieName = ConvertPropertyNameToCookieName(propertyName);

			var resp = Context.Response;
			if (resp != null)
			{
				var cookie = Harmless_Get(resp.Cookies, cookieName);
				if (cookie != null)
				{
					return cookie;
				}
			}

			var req = Context.Request;
			if (req != null)
			{
				var cookie = Harmless_Get(req.Cookies, cookieName);
				if (cookie != null)
				{
					return cookie;
				}
			}
			return null;
		}

		private HttpCookie CreateRawCookie(string propertyName)
		{
			// propertyName will be replace if settings exist
			var newCookie = new HttpCookie(propertyName);

			if (DefaultDomain != null)
			{
				newCookie.Domain = DefaultDomain;
			}

			if (DefaultExpires.HasValue)
			{
				newCookie.Expires = DateTime.UtcNow.Add(DefaultExpires.Value);
			}

			if (DefaultSecure.HasValue)
			{
				newCookie.Secure = DefaultSecure.Value;
			}

			FastCookieSettingsAttribute setting = GetPropertySettings(propertyName);
			if (setting == null)
				return newCookie;

			if (setting.Name != null)
				newCookie.Name = setting.Name;
			if (setting.ExpiresInDays > 0)
				newCookie.Expires = DateTime.UtcNow.AddDays(setting.ExpiresInDays);
			if (!string.IsNullOrWhiteSpace(setting.Domain))
				newCookie.Domain = setting.Domain;
			if (!string.IsNullOrWhiteSpace(setting.Path))
				newCookie.Path = setting.Path;
			newCookie.HttpOnly = setting.HttpOnly;
			newCookie.Secure = setting.Secure;

			return newCookie;
		}

		private void Internal_SetResponseCookie(HttpCookie cookie)
		{
			if (Context == null || Context.Response == null)
				return;

			var cookies = Context.Response.Cookies;
			if (cookies.AllKeys.Contains(cookie.Name, StringComparer.InvariantCultureIgnoreCase))
			{
				// If this has been set in the response already, 
				// remove the previous setting and replace with existing one
				cookies.Remove(cookie.Name);
			}

			cookies.Add(cookie);
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
				if(DateTime.TryParse(value, out dt))
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
