using CurrencyCalculator.Core;
using System.Collections.Generic;

namespace CurrencyCalculator.API
{
	public class TotalRequest
	{
		public string ToCurrencyId { get; set; } 
		
		public IEnumerable<Amount> Amounts { get; set; }
	}
}
