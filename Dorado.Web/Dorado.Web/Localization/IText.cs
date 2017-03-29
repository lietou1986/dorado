using System;

namespace Dorado.Web.Localization
{
	public interface IText
	{
		LocalizedString Get(string key, params object[] args);
	}
}
