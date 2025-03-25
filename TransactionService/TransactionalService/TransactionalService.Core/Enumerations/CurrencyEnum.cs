using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransactionalService.Core.Enumerations
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum CurrencyEnum
	{
		USD,
		EUR,
		GBP,
		INR,
		AUD,
		CAD,
		SGD,
		CHF,
		MYR,
		JPY,
		CNY,
		VND
	}
}
