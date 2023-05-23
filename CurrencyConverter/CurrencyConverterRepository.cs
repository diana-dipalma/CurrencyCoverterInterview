using System;
using System.Collections.Generic;

namespace CurrencyConverter
{
    public class CurrencyConverterRepository : ICurrencyConverterRepository
    {
        public IEnumerable<CurrencyConversion> GetConversions()
        {
            //RateFromUSD may be outdated values
            return new[] {
                new CurrencyConversion() { CurrencyCode = "USD", CurrencyName = "United States Dollars", RateFromUSDToCurrency = 1.0M },
                new CurrencyConversion() { CurrencyCode = "CAD", CurrencyName = "Canada Dollars", RateFromUSDToCurrency = 1.27M },
                new CurrencyConversion() { CurrencyCode = "MXN", CurrencyName = "Mexico Pesos", RateFromUSDToCurrency = 20.56M },
                new CurrencyConversion() { CurrencyCode = "CRC", CurrencyName = "Costa Rica Colons", RateFromUSDToCurrency = 642.83M },
                new CurrencyConversion() { CurrencyCode = "DZD", CurrencyName = "Algeria Dinars", RateFromUSDToCurrency = 140.26M },
                new CurrencyConversion() { CurrencyCode = "CNY", CurrencyName = "China Renminbis", RateFromUSDToCurrency = 6.35M },
                new CurrencyConversion() { CurrencyCode = "DKK", CurrencyName = "Denmark Krones", RateFromUSDToCurrency = 6.52M },
                new CurrencyConversion() { CurrencyCode = "PLN", CurrencyName = "Poland Zlotys", RateFromUSDToCurrency = 3.95M }
            };
        }
    }
}
