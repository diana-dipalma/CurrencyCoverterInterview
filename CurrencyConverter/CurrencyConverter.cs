using System;
using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<string, CurrencyInfo> _supportedCurrencyInfos;
        private Dictionary<CachedRateKey, Decimal> _cachedRates;

        private Dictionary<string, HashSet<string>> _knownDirectRatesFromCurrency;
        private Dictionary<string, HashSet<string>> _knownDirectRatesToCurrency;

        private readonly object _lock = new object();

        // Initialize a CurrencyConvert using the default repository of USD conversions.
        // Reciprocal rates are automatically added.
        public CurrencyConverter()
            : this(new CurrencyConverterRepository()) { }

        // Initialize a CurrencyConverter using the given repository of conversions.
        // This is used to inject a mock repository for testing.
        public CurrencyConverter(ICurrencyConverterRepository repository)
        {
            _supportedCurrencyInfos = new Dictionary<string, CurrencyInfo>();
            _cachedRates = new Dictionary<CachedRateKey, Decimal>();
            _knownDirectRatesFromCurrency = new Dictionary<string, HashSet<string>>();
            _knownDirectRatesToCurrency = new Dictionary<string, HashSet<string>>();
            var usd_conversions = repository.GetConversions();
            ProcessUpdate(new CurrencyUpdate(usd_conversions));
        }

        // Legacy method for converting currencies, uses a direct rate if available, otherwise
        // attempts to convert through USD.
        public Decimal GetConvertedAmount(string fromCurrency, string toCurrency, Decimal amount)
        {
            return GetConvertedAmount(fromCurrency, toCurrency, new[] { "USD" }, amount);
        }

        private Decimal GetConvertedAmountInternal(
            string fromCurrency,
            string toCurrency,
            IEnumerable<string> intermediateCurrencies,
            Decimal amount
        )
        {
            if (!_supportedCurrencyInfos.ContainsKey(fromCurrency))
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(fromCurrency)}={fromCurrency}"
                );
            }
            if (!_supportedCurrencyInfos.ContainsKey(toCurrency))
            {
                throw new ArgumentException(
                    $"Unsupported currency: {nameof(toCurrency)}={toCurrency}"
                );
            }
            foreach (var intermediate in intermediateCurrencies)
            {
                if (!_supportedCurrencyInfos.ContainsKey(intermediate))
                {
                    throw new ArgumentException(
                        $"Unsupported intermediate currency: {intermediate}"
                    );
                }
            }
            var directRateKey = new CachedRateKey()
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                IntermediateCurrency = null
            };
            // First preference is a direct rate
            // (null intermediate currency in cached rate key)
            string[] direct = { null };
            string[] intermediates = direct.Concat(intermediateCurrencies).ToArray();
            foreach (var intermediate in intermediates)
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
                    return RoundUsingCurrencyRules(amount * rate, toCurrency);
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

        // Convert currencies, preferring direct rates, then falling back on the intermediate
        // currencies, using the first one that is available.
        //
        // This uses a lock to avoid conflicting with ProcessUpdate.
        public Decimal GetConvertedAmount(
            string fromCurrency,
            string toCurrency,
            IEnumerable<string> intermediateCurrencies,
            Decimal amount
        )
        {
            lock (_lock)
            {
                return GetConvertedAmountInternal(
                    fromCurrency,
                    toCurrency,
                    intermediateCurrencies,
                    amount
                );
            }
        }

        // Retrieve the name of a currency given its code.
        //
        // This uses a lock to avoid conflicting with ProcessUpdate.
        public string GetCurrencyName(string currencyCode)
        {
            lock (_lock)
            {
                if (_supportedCurrencyInfos.TryGetValue(currencyCode, out CurrencyInfo info))
                {
                    return info.CurrencyName;
                }
                else
                {
                    throw new ArgumentException(
                        $"Unsupported currency: {nameof(currencyCode)}={currencyCode}"
                    );
                }
            }
        }

        // Process an update to the supported currencies and conversion rates.
        // Reciprocal rates are automatically added and conversion rates through intermediate
        // currencies are cached.
        //
        // This uses a lock to avoid conflicting with GetConvertedAmount and GetCurrencyName.
        public void ProcessUpdate(CurrencyUpdate update)
        {
            lock (_lock)
            {
                if (update.Deletions != null)
                {
                    foreach (var code in update.Deletions)
                    {
                        ProcessDeletion(code);
                    }
                }
                if (update.CurrencyInfos != null)
                {
                    foreach (var info in update.CurrencyInfos)
                    {
                        ProcessInfo(info);
                    }
                }
                if (update.UpdatedConversions != null)
                {
                    var updatedCurrencies = ProcessUpdatedConversions(update.UpdatedConversions);
                    foreach (var code in updatedCurrencies)
                    {
                        UpdateIntermediateConversions(code);
                    }
                }
            }
        }

        private void ProcessDeletion(string code)
        {
            if (!_supportedCurrencyInfos.Remove(code))
            {
                throw new ArgumentException(
                    $"Currency {code} was not supported, cannot delete it."
                );
            }
        }

        private void ProcessInfo(CurrencyInfo info)
        {
            var code = info.CurrencyCode;
            var name = info.CurrencyName;
            var decimalDigits = info.DecimalDigits;
            var roundingIncrement = info.RoundingIncrement;
            if (!IsValidCurrencyCode(code))
            {
                throw new ArgumentException(
                    $"Currency codes must be 3 uppercase alpha characters: {code}"
                );
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"Invalid name for currency {code}: {name}");
            }
            // This condition matches if both are null or neither is null
            if ((decimalDigits == null) == (roundingIncrement == null))
            {
                throw new ArgumentException(
                    $"Exactly one of decimalDigits and roundingIncrement must be set in "
                        + $"currency info for {code}: decimalDigits={decimalDigits}, "
                        + $"roundingIncrement={roundingIncrement}"
                );
            }
            if (decimalDigits != null && (decimalDigits.Value < 0))
            {
                throw new ArgumentException(
                    $"Invalid decimalDigits for currency {code}: {decimalDigits.Value}"
                );
            }
            if (roundingIncrement != null && (roundingIncrement.Value <= 0))
            {
                throw new ArgumentException(
                    $"Invalid roundingIncrement for currency {code}: {roundingIncrement.Value}"
                );
            }
            _supportedCurrencyInfos[code] = info;
        }

        private HashSet<string> ProcessUpdatedConversions(IEnumerable<CurrencyConversionPair> pairs)
        {
            var updatedCurrencies = new HashSet<string>();
            foreach (var pair in pairs)
            {
                if (!_supportedCurrencyInfos.ContainsKey(pair.FromCurrency))
                {
                    throw new ArgumentException(
                        $"FromCurrency {pair.FromCurrency} is not supported, cannot process update."
                    );
                }
                if (!_supportedCurrencyInfos.ContainsKey(pair.ToCurrency))
                {
                    throw new ArgumentException(
                        $"ToCurrency {pair.ToCurrency} is not supported, cannot process update."
                    );
                }
                if (pair.Rate <= 0)
                {
                    throw new ArgumentException(
                        $"Invalid rate for conversion from {pair.FromCurrency} to "
                            + $"{pair.ToCurrency}: {pair.Rate}"
                    );
                }
                var cachedRateKey = new CachedRateKey()
                {
                    FromCurrency = pair.FromCurrency,
                    ToCurrency = pair.ToCurrency,
                    IntermediateCurrency = null
                };
                _cachedRates[cachedRateKey] = pair.Rate;

                var reversedRateKey = new CachedRateKey()
                {
                    FromCurrency = pair.ToCurrency,
                    ToCurrency = pair.FromCurrency,
                    IntermediateCurrency = null
                };
                _cachedRates[reversedRateKey] = 1.0M / pair.Rate;
                updatedCurrencies.Add(pair.FromCurrency);
                updatedCurrencies.Add(pair.ToCurrency);
                UpdateKnownRatePair(pair.FromCurrency, pair.ToCurrency);
                UpdateKnownRatePair(pair.ToCurrency, pair.FromCurrency);
            }
            return updatedCurrencies;
        }

        private void UpdateKnownRatePair(string from, string to)
        {
            if (!_knownDirectRatesFromCurrency.TryGetValue(from, out HashSet<string> fromRates))
            {
                fromRates = new HashSet<string>();
                _knownDirectRatesFromCurrency[from] = fromRates;
            }
            fromRates.Add(to);

            if (!_knownDirectRatesToCurrency.TryGetValue(to, out HashSet<string> toRates))
            {
                toRates = new HashSet<string>();
                _knownDirectRatesToCurrency[to] = toRates;
            }
            toRates.Add(from);
        }

        private void UpdateIntermediateConversions(string code)
        {
            if (!_knownDirectRatesToCurrency.TryGetValue(code, out HashSet<string> toRates))
            {
                return;
            }
            if (!_knownDirectRatesFromCurrency.TryGetValue(code, out HashSet<string> fromRates))
            {
                return;
            }
            foreach (var fromCurrency in fromRates)
            {
                var rateKey1 = new CachedRateKey()
                {
                    FromCurrency = fromCurrency,
                    ToCurrency = code,
                    IntermediateCurrency = null
                };
                var rate1 = GetConversionRate(rateKey1);
                foreach (var toCurrency in toRates)
                {
                    var rateKey2 = new CachedRateKey()
                    {
                        FromCurrency = code,
                        ToCurrency = toCurrency,
                        IntermediateCurrency = null
                    };
                    var rate2 = GetConversionRate(rateKey2);
                    var rateKey = new CachedRateKey()
                    {
                        FromCurrency = fromCurrency,
                        ToCurrency = toCurrency,
                        IntermediateCurrency = code
                    };
                    _cachedRates[rateKey] = rate1 * rate2;
                }
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
                    $"Conversion rate from {cachedRateKey.FromCurrency} to "
                        + $"{cachedRateKey.ToCurrency} via {cachedRateKey.IntermediateCurrency} "
                        + $"does not exist."
                );
            }
        }

        private Decimal RoundUsingCurrencyRules(Decimal amount, string currencyCode)
        {
            if (!_supportedCurrencyInfos.TryGetValue(currencyCode, out CurrencyInfo info))
            {
                throw new InvalidOperationException(
                    $"Currency {currencyCode} is not supported, cannot round."
                );
            }
            if (info.DecimalDigits != null)
            {
                return Math.Round(amount, info.DecimalDigits.Value);
            }
            else
            {
                return Math.Round(amount / info.RoundingIncrement.Value)
                    * info.RoundingIncrement.Value;
            }
        }
    }
}
