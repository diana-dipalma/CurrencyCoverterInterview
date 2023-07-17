using System;
using System.Collections.Generic;

namespace CurrencyConverter
{
    public class CurrencyConverterRepository
    {
        public IEnumerable<CurrencyConversion> GetConversions()
        {
            //RateFromUSD may be outdated values
            return new[] {
                new CurrencyConversion() { CountryCode = "USD", CurrencyName = "United States Dollars", RateFromUSDToCurrency = 1.0M },
                new CurrencyConversion() { CountryCode = "CAD", CurrencyName = "Canada Dollars", RateFromUSDToCurrency = 1.27M },
                new CurrencyConversion() { CountryCode = "MXN", CurrencyName = "Mexico Pesos", RateFromUSDToCurrency = 20.56M },
                new CurrencyConversion() { CountryCode = "CRC", CurrencyName = "Costa Rica Colons", RateFromUSDToCurrency = 642.83M },
                new CurrencyConversion() { CountryCode = "DZD", CurrencyName = "Algeria Dinars", RateFromUSDToCurrency = 140.26M },
                new CurrencyConversion() { CountryCode = "CNY", CurrencyName = "China Renminbis", RateFromUSDToCurrency = 6.35M },
                new CurrencyConversion() { CountryCode = "DKK", CurrencyName = "Denmark Krones", RateFromUSDToCurrency = 6.52M },
                new CurrencyConversion() { CountryCode = "PLN", CurrencyName = "Poland Zlotys", RateFromUSDToCurrency = 3.95M },
            };
        }
    }
}
