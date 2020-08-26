using SweaWebService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CurrencyCalculator.Core;

namespace CurrencyCalculator.Infrastructure
{
	public class CurrencyService : ICurrencyService
	{
		private readonly SweaWebServicePortType _serviceClient;

		public CurrencyService(SweaWebServicePortType serviceClient)
		{
			_serviceClient = serviceClient;
		}

		public async Task<double> ExchangeRate(string fromCurrencyId, string toCurrencyId, DateTime exchangeDate)
		{
			var response = await _serviceClient.getCrossRatesAsync(new getCrossRatesRequest(new CrossRequestParameters { crossPair = new CurrencyCrossPair[] { new CurrencyCrossPair { seriesid1 = fromCurrencyId, seriesid2 = toCurrencyId } }, datefrom = exchangeDate, dateto = exchangeDate, aggregateMethod = AggregateMethodType.D, languageid = LanguageType.en }));

			foreach(var serie in response.@return.groups[0].series)
			{
				if(serie.seriesid1 == fromCurrencyId && serie.seriesid2 == toCurrencyId)
				{
					return serie.resultrows[0].value ?? 0;
				}
			}

			return 0;
		}

		public async Task<IEnumerable<Currency>> Currencies()
		{
			var response = await _serviceClient.getAllCrossNamesAsync(new getAllCrossNamesRequest { languageid = LanguageType.en });

			return from r in response.@return select new Currency { Code = r.seriesname, Id = r.seriesid, Name = r.seriesdescription };
		}

		public async Task<DateTime> LatestExchangeDate(string toCurrencyId)
		{
			var response = await _serviceClient.getLatestInterestAndExchangeRatesAsync(new getLatestInterestAndExchangeRatesRequest(LanguageType.en, new string[] { toCurrencyId }));

			return response.@return.groups[0].series[0].resultrows[0].date ?? DateTime.Now;
		}
	}
}
