namespace CurrencyCalculator.Core
{
	public class ConvertedAmount
	{
		public ConvertedAmount(double value, double exchangeRate)
		{
			Value = value;
			ExchangeRate = exchangeRate;
		}

		public double Value { get; private set; }

		public double ExchangeRate { get; private set; }
	}
}
