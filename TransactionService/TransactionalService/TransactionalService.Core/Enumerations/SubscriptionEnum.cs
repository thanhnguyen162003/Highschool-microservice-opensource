using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransactionalService.Core.Enumerations
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum SubscriptionEnum
	{
		active,
		canceled,
		expired,
		trial
	}
}
