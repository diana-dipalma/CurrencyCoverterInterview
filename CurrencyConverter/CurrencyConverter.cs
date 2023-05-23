using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CurrencyConverter
{
    public class CurrencyConverter
    {
        private struct CachedRateKey
        {
            public string FromCurrency;
            public string ToCurrency;
            public string IntermediateCurrency;
        }

        private readonly Dictionary<string, string> _supportedCurrencyNames;
        private readonly Dictionary<CachedRateKey, Decimal> _cachedRates;

        public CurrencyConverter()
            : this(new CurrencyConverterRepository()) { }

        public CurrencyConverter(ICurrencyConverterRepository repository)
        {
            _supportedCurrencyNames = new Dictionary<string, string>();
            _cachedRates = new Dictionary<CachedRateKey, Decimal>();
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
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException($"Invalid name for currency {code}: {name}");
                }
                _supportedCurrencyNames[conv1.CurrencyCode] = conv1.CurrencyName;
                foreach (var conv2 in usd_conversions)
                {
                    Decimal pairRate = conv2.RateFromUSDToCurrency / conv1.RateFromUSDToCurrency;
                    var cachedRateKey = new CachedRateKey()
                    {
                        FromCurrency = conv1.CurrencyCode,
                        ToCurrency = conv2.CurrencyCode,
                        IntermediateCurrency = "USD"
                    };
                    _cachedRates[cachedRateKey] = pairRate;
                }
            }
        }

        public Decimal GetConvertedAmount(string fromCurrency, string toCurrency, Decimal amount)
        {
            return GetConvertedAmount(fromCurrency, toCurrency, new[] { "USD" }, amount);
        }

        public Decimal GetConvertedAmount(
            string fromCurrency,
            string toCurrency,
            IEnumerable<string> intermediateCurrencies,
            Decimal amount
        )
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
            foreach (var intermediate in intermediateCurrencies)
            {
                if (!_supportedCurrencyNames.ContainsKey(intermediate))
                {
                    throw new ArgumentException(
                        $"Unsupported intermediate currency: {intermediate}"
                    );
                }
            }
            foreach (var intermediate in intermediateCurrencies)
            {
                var cachedRateKey = new CachedRateKey()
                {
                    FromCurrency = fromCurrency,
                    ToCurrency = toCurrency,
                    IntermediateCurrency = intermediate
                };

                if (_cachedRates.ContainsKey(cachedRateKey))
                {
                    var rate = GetConversionRate(cachedRateKey);
                    return Math.Round(amount * rate, 2);
                }
            }
            throw new ArgumentException(
                string.Format(
                    "No conversion rate found for {0} to {1} via any of [{2}]",
                    fromCurrency,
                    toCurrency,
                    string.Join(", ", intermediateCurrencies)
                )
            );
        }

        public string GetCurrencyName(string currencyCode)
        {
            if (_supportedCurrencyNames.TryGetValue(currencyCode, out string currencyName))
            {
                return currencyName;
            }
            else
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(currencyCode)}={currencyCode}"
                );
            }
        }

        private bool IsValidCurrencyCode(string code)
        {
            return code != null && code.Length == 3 && Regex.IsMatch(code, @"^[A-Z]+$");
        }

        private Decimal GetConversionRate(CachedRateKey cachedRateKey)
        {
            if (_cachedRates.TryGetValue(cachedRateKey, out decimal rate))
            {
                return rate;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Conversion rate from {cachedRateKey.FromCurrency} to {cachedRateKey.ToCurrency} via {cachedRateKey.IntermediateCurrency} does not exist."
                );
            }
        }
    }
}
