using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CurrencyConverter
{
    public class CurrencyConverter
    {
        private readonly Dictionary<string, string> _supportedCurrencyNames;
        private readonly Dictionary<(string, string), Decimal> _conversions;

        public CurrencyConverter()
            : this(new CurrencyConverterRepository()) { }

        public CurrencyConverter(ICurrencyConverterRepository repository)
        {
            _supportedCurrencyNames = new Dictionary<string, string>();
            _conversions = new Dictionary<(string, string), Decimal>();
            var usd_conversions = repository.GetConversions();
            foreach (var conv1 in usd_conversions)
            {
                var code = conv1.CurrencyCode;
                var name = conv1.CurrencyName;
                var usdRate = conv1.RateFromUSDToCurrency;
                if (!IsValidCurrencyCode(code))
                {
                    throw new ArgumentException(
                        $"Currency codes must be 3 uppercase alpha characters: {code}"
                    );
                }
                if (usdRate <= 0)
                {
                    throw new ArgumentException($"Invalid rate for currency {code}: {usdRate}");
                }
                if (String.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException($"Invalid name for currency {code}: {name}");
                }
                _supportedCurrencyNames[conv1.CurrencyCode] = conv1.CurrencyName;
                foreach (var conv2 in usd_conversions)
                {
                    Decimal pairRate = conv2.RateFromUSDToCurrency / conv1.RateFromUSDToCurrency;
                    _conversions[(conv1.CurrencyCode, conv2.CurrencyCode)] = pairRate;
                }
            }
        }

        public Decimal GetConvertedAmount(string fromCurrency, string toCurrency, Decimal amount)
        {
            if (!_supportedCurrencyNames.ContainsKey(fromCurrency))
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(fromCurrency)}={fromCurrency}"
                );
            }
            if (!_supportedCurrencyNames.ContainsKey(toCurrency))
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(toCurrency)}={toCurrency}"
                );
            }
            var rate = GetConversionRate(fromCurrency, toCurrency);
            return Math.Round(amount * rate, 2);
        }

        public string GetCurrencyName(string currencyCode)
        {
            if (!_supportedCurrencyNames.ContainsKey(currencyCode))
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(currencyCode)}={currencyCode}"
                );
            }
            return _supportedCurrencyNames[currencyCode];
        }

        private bool IsValidCurrencyCode(string code)
        {
            return code != null && code.Length == 3 && Regex.IsMatch(code, @"^[A-Z]+$");
        }

        private Decimal GetConversionRate(string fromCurrency, string toCurrency)
        {
            return _conversions[(fromCurrency, toCurrency)];
        }
    }
}
