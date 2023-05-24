using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyConverter
{
    public class CurrencyUpdate
    {
        public CurrencyUpdate(
            IEnumerable<string> deletions,
            IEnumerable<CurrencyInfo> currencyInfos,
            IEnumerable<CurrencyConversionPair> updatedConversions
        )
        {
            _deletions = deletions;
            _currencyInfos = currencyInfos;
            _updatedConversions = updatedConversions;
        }

        public CurrencyUpdate(IEnumerable<CurrencyConversion> legacyUsdConversions)
            : this(
                new string[0],
                CurrencyInfosFromLegacyUsd(legacyUsdConversions),
                CurrencyConversionPairsFromLegacyUsd(legacyUsdConversions)
            )
        { }

        public override string ToString()
        {
            return $"Deletions: {string.Join(", ", _deletions)}\n"
                + $"CurrencyInfos: {string.Join(", ", _currencyInfos)}\n"
                + $"UpdatedConversions: {string.Join(", ", _updatedConversions)}";
        }

        public bool Equals(CurrencyUpdate other)
        {
            return _deletions.SequenceEqual(other._deletions)
                && _currencyInfos.SequenceEqual(other._currencyInfos)
                && _updatedConversions.SequenceEqual(other._updatedConversions);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CurrencyUpdate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_deletions, _currencyInfos, _updatedConversions);
        }

        public IEnumerable<string> Deletions
        {
            get { return _deletions; }
        }

        public IEnumerable<CurrencyInfo> CurrencyInfos
        {
            get { return _currencyInfos; }
        }

        public IEnumerable<CurrencyConversionPair> UpdatedConversions
        {
            get { return _updatedConversions; }
        }

        private static IEnumerable<CurrencyInfo> CurrencyInfosFromLegacyUsd(
            IEnumerable<CurrencyConversion> legacyUsdConversions
        )
        {
            var currencyInfos = new List<CurrencyInfo>();
            foreach (var conv in legacyUsdConversions)
            {
                currencyInfos.Add(new CurrencyInfo(conv.CurrencyCode, conv.CurrencyName, 2, null));
            }
            return currencyInfos;
        }

        private static IEnumerable<CurrencyConversionPair> CurrencyConversionPairsFromLegacyUsd(
            IEnumerable<CurrencyConversion> legacyUsdConversions
        )
        {
            var currencyPairs = new List<CurrencyConversionPair>();
            foreach (var conv in legacyUsdConversions)
            {
                currencyPairs.Add(
                    new CurrencyConversionPair("USD", conv.CurrencyCode, conv.RateFromUSDToCurrency)
                );
            }
            return currencyPairs;
        }

        private IEnumerable<string> _deletions;
        private IEnumerable<CurrencyInfo> _currencyInfos;
        private IEnumerable<CurrencyConversionPair> _updatedConversions;
    }
}
