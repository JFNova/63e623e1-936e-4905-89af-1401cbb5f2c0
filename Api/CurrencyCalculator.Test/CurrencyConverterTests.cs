using CurrencyCalculator.Core;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyCalculator.Test
{
	[TestClass]
	public class CurrencyConverterTests
	{
		private ICurrencyService _service;

		private const string _currencyIdSek = "SEK";
		private const string _currencyIdUsd = "SEKUSDPMI";
		private const string _currencyIdEur = "SEKEURPMI";

		private Currency _currencySek = new Currency { Id = _currencyIdSek, Code = "SEK", Name = "Swedish Krona" };
		private Currency _currencyUsd = new Currency { Id = _currencyIdUsd, Code = "USD", Name = "US Dollar" };
		private Currency _currencyEur = new Currency { Id = _currencyIdEur, Code = "Eur", Name = "Euro" };

		[TestInitialize]
		public void TestInitialize()
		{
			_service = A.Fake<ICurrencyService>();
			A.CallTo(() => _service.Currencies()).Returns(new List<Currency>() { _currencySek, _currencyUsd, _currencyEur });
		}

		[TestMethod]
		public async Task ShouldConvert()
		{
			// Given
			var converter = new CurrencyConverter(_service);
			
			var exchangeDate = new DateTime(2015, 04, 23);
			var fromAmount = 100;
			var exchangeRate = 0.1143;
			var expected = 11.43;

			// When
			A.CallTo(() => _service.ExchangeRate(_currencyIdSek, _currencyIdUsd, exchangeDate)).Returns(exchangeRate);
			
			var actual = await converter.ConvertAmount(_currencyIdSek, _currencyIdUsd, exchangeDate, fromAmount);

			// Then
			Assert.AreEqual(expected, actual.Value);
		}

		[TestMethod]
		public async Task ShouldCalculateTotal()
		{
			// Given
			var converter = new CurrencyConverter(_service);
			
			var toCurrencyId = _currencyIdUsd;

			var latestExchangeDate = DateTime.Now.Date;
			
			var latestExchangeRateSekUsd = 0.1145;
			var latestExchangeRateEurUsd = 1.1839;
			
			var fromAmountSek = 100;
			var fromAmountEur = 5;

			var expected = latestExchangeRateSekUsd * fromAmountSek + latestExchangeRateEurUsd * fromAmountEur;

			// When
			A.CallTo(() => _service.LatestExchangeDate(toCurrencyId)).Returns(latestExchangeDate);

			A.CallTo(() => _service.ExchangeRate(_currencyIdSek, toCurrencyId, latestExchangeDate)).Returns(latestExchangeRateSekUsd);
			A.CallTo(() => _service.ExchangeRate(_currencyIdEur, toCurrencyId, latestExchangeDate)).Returns(latestExchangeRateEurUsd);

			var actual = await converter.TotalAmount(toCurrencyId, new List<Amount> { new Amount { Value = fromAmountSek, CurrencyId = _currencyIdSek }, new Amount { Value = fromAmountEur, CurrencyId = _currencyIdEur } });

			// Then
			Assert.AreEqual(expected, actual);
		}
	}
}
