using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyCalculator.Core
{
	public interface ICurrencyService
	{
		public Task<double> ExchangeRate(string fromCurrencyId, string toCurrencyId, DateTime exchangeDate);

		public Task<DateTime> LatestExchangeDate(string currencyId);

		public Task<IEnumerable<Currency>> Currencies();
	}
}
