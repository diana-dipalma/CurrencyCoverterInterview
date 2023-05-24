using System;

namespace CurrencyConverter
{
    public class CurrencyInfo
    {
        public CurrencyInfo(
            string currencyCode,
            string currencyName,
            Nullable<int> decimalDigits,
            Nullable<Decimal> roundingIncrement
        )
        {
            _currencyCode = currencyCode;
            _currencyName = currencyName;
            _decimalDigits = decimalDigits;
            _roundingIncrement = roundingIncrement;
        }

        public override string ToString()
        {
            return $"{_currencyCode} {_currencyName} {_decimalDigits} {_roundingIncrement}";
        }

        public bool Equals(CurrencyInfo other)
        {
            return _currencyCode.Equals(other._currencyCode)
                && _currencyName.Equals(other._currencyName)
                && _decimalDigits.Equals(other._decimalDigits)
                && _roundingIncrement.Equals(other._roundingIncrement);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CurrencyInfo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_currencyCode, _currencyName, _decimalDigits, _roundingIncrement);
        }

        public string CurrencyCode
        {
            get { return _currencyCode; }
        }

        public string CurrencyName
        {
            get { return _currencyName; }
        }

        public Nullable<int> DecimalDigits
        {
            get { return _decimalDigits; }
        }

        public Nullable<Decimal> RoundingIncrement
        {
            get { return _roundingIncrement; }
        }

        // Required and must be 3 uppercase alpha characters ([A-Z])
        private readonly string _currencyCode;

        // Required and must be non-empty
        private readonly string _currencyName;

        // At most one of these can be set, and if set it will determine the rounding behavior.
        // Either if set must be a positive value. If neither is set, the default will be rounding
        // to 2 decimal places (equivalent to setting DecimalDigits to 2 or RoundIncrement to 0.01).
        private readonly Nullable<int> _decimalDigits;
        private readonly Nullable<Decimal> _roundingIncrement;
    }
}
