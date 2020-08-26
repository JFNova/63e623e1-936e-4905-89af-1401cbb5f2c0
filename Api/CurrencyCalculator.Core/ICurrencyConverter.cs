using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyCalculator.Core
{
	public interface ICurrencyConverter
	{
		Task<ConvertedAmount> ConvertAmount(string fromCurrencyId, string toCurrencyId, DateTime exchangeDate, double amount);

		Task<double> TotalAmount(string toCurrencyId, IEnumerable<Amount> fromAmountsWithCurrencyId);
	}
}
