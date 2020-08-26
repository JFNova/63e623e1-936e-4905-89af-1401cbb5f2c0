using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyCalculator.Core
{
	public class CurrencyConverter : ICurrencyConverter
	{
		private readonly ICurrencyService _currencyService;
		
		public CurrencyConverter(ICurrencyService currencyService)
		{
			_currencyService = currencyService;
		}

		public async Task<ConvertedAmount> ConvertAmount(string fromCurrencyId, string toCurrencyId, DateTime exchangeDate, double amount)
		{
			var exchangeRate = await _currencyService.ExchangeRate(fromCurrencyId, toCurrencyId, exchangeDate);
			var convertedAmount =  amount * exchangeRate;
			
			return new ConvertedAmount(convertedAmount, exchangeRate);
		}

		public async Task<double> TotalAmount(string toCurrencyId, IEnumerable<Amount> fromAmountsWithCurrencyId)
		{
			double sum = 0;
			var latestDate = await _currencyService.LatestExchangeDate(toCurrencyId);

			foreach(var amount in fromAmountsWithCurrencyId)
			{
				var convertedAmount = await ConvertAmount(amount.CurrencyId, toCurrencyId, latestDate, amount.Value);
				sum += convertedAmount.Value;
			}

			return sum;
		}
	}
}
