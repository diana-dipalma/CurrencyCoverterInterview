using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CurrencyConverter
{
    public class CurrencyConverter
    {
        private readonly HashSet<string> _supportedCurrencies;
        private readonly Dictionary<(string, string), Decimal> _conversions;

        public CurrencyConverter()
            : this(new CurrencyConverterRepository()) { }

        public CurrencyConverter(ICurrencyConverterRepository repository)
        {
            _supportedCurrencies = new HashSet<string>();
            _conversions = new Dictionary<(string, string), Decimal>();
            var usd_conversions = repository.GetConversions();
            foreach (var conv1 in usd_conversions)
            {
                var code = conv1.CountryCode;
                if (!IsValidCurrencyCode(code))
                {
                    throw new ArgumentException(
                        $"Currency codes must be 3 uppercase alpha characters: {code}"
                    );
                }
                if (conv1.RateFromUSDToCurrency <= 0)
                {
                    throw new ArgumentException($"Invalid rate for currency: {conv1.CountryCode}");
                }
                _supportedCurrencies.Add(conv1.CountryCode);
                foreach (var conv2 in usd_conversions)
                {
                    Decimal rate = conv2.RateFromUSDToCurrency / conv1.RateFromUSDToCurrency;
                    _conversions[(conv1.CountryCode, conv2.CountryCode)] = rate;
                }
            }
        }

        public Decimal GetConvertedAmount(string fromCurrency, string toCurrency, Decimal amount)
        {
            if (!_supportedCurrencies.Contains(fromCurrency))
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(fromCurrency)}={fromCurrency}"
                );
            }
            if (!_supportedCurrencies.Contains(toCurrency))
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(toCurrency)}={toCurrency}"
                );
            }
            var rate = GetConversionRate(fromCurrency, toCurrency);
            return Math.Round(amount * rate, 2);
        }

        private bool IsValidCurrencyCode(string code)
        {
            return code.Length == 3 && Regex.IsMatch(code, @"^[A-Z]+$");
        }

        private Decimal GetConversionRate(string fromCurrency, string toCurrency)
        {
            return _conversions[(fromCurrency, toCurrency)];
        }
    }
}
