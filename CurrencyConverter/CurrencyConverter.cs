using System;
using System.Collections.Generic;

namespace CurrencyConverter
{
    public class CurrencyConverter
    {
        private readonly Dictionary<(string, string), Decimal> _conversions;

        public CurrencyConverter()
        {
            _conversions = new Dictionary<(string, string), Decimal>();
            var usd_conversions = new CurrencyConverterRepository().GetConversions();
            foreach (var conv1 in usd_conversions)
            {
                foreach (var conv2 in usd_conversions)
                {
                    Decimal rate = conv2.RateFromUSDToCurrency / conv1.RateFromUSDToCurrency;
                    _conversions[(conv1.CountryCode, conv2.CountryCode)] = rate;
                }
            }
        }

        public Decimal GetConvertedAmount(string fromCurrency, string toCurrency, Decimal amount)
        {
            var rate = GetConversionRate(fromCurrency, toCurrency);
            return Math.Round(amount * rate, 2);
        }

        private Decimal GetConversionRate(string fromCurrency, string toCurrency)
        {
            return _conversions[(fromCurrency, toCurrency)];
        }
    }
}
