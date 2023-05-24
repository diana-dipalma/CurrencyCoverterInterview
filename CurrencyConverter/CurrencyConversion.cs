using System;
namespace CurrencyConverter
{
    public class CurrencyConversion
    {
        // [DEPRECATED] This is internally converted to CurrencyInfo and CurrencyConversionPair.
        // Remove this when the initial base rates for currency conversion are migrated to use
        // CurrencyUpdate directly.
        public String CurrencyCode;
        public String CurrencyName;
        public Decimal RateFromUSDToCurrency;
    }
}
