using System;
namespace CurrencyConverter
{
    public class CurrencyConversionPair
    {
        public CurrencyConversionPair(string fromCurrency, string toCurrency, Decimal rate)
        {
            _fromCurrency = fromCurrency;
            _toCurrency = toCurrency;
            _rate = rate;
        }

        public override string ToString()
        {
            return $"{_fromCurrency} to {_toCurrency} at {_rate}";
        }

        public bool Equals(CurrencyConversionPair other)
        {
            return _fromCurrency.Equals(other._fromCurrency)
                && _toCurrency.Equals(other._toCurrency)
                && _rate.Equals(other._rate);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CurrencyConversionPair);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_fromCurrency, _toCurrency, _rate);
        }

        public string FromCurrency
        {
            get { return _fromCurrency; }
        }

        public string ToCurrency
        {
            get { return _toCurrency; }
        }

        public Decimal Rate
        {
            get { return _rate; }
        }

        private string _fromCurrency;
        private string _toCurrency;
        private Decimal _rate;
    }
}