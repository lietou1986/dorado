using System;
using Dorado.Core;
using Dorado.Core.ComponentModel;
using Dorado.Web.Localization;

namespace Dorado.Web.Filters
{

	public enum NotifyType
	{
		Info,
		Success,
		Warning,
		Error
	}

	[Serializable]
	public class NotifyEntry : ComparableObject<NotifyEntry>
	{
		[ObjectSignature]
		public NotifyType Type { get; set; }

		[ObjectSignature]
		public LocalizedString Message { get; set; }

		public bool Durable { get; set; }
	}

}
