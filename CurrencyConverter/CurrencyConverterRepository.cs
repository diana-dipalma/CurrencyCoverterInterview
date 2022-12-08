using System;
using System.Collections.Generic;

namespace CurrencyConverter
{
    public class CurrencyConverterRepository
    {
        public Dictionary<String, CurrencyConversion> GetConversions()
        {
            //RateFromUSD may be outdated values
            //Changed from IEumerate to Dictionary to get close to O(1) time when accessing each CurrencyConversion object
            Dictionary<String, CurrencyConversion> Currency = new Dictionary<String, CurrencyConversion>();
            Currency["USD"] = new CurrencyConversion() { CountryCode = "USD", CurrencyName = "United States Dollars", RateFromUSDToCurrency = 1.0M };
            Currency["CAD"] = new CurrencyConversion() { CountryCode = "CAD", CurrencyName = "Canada Dollars", RateFromUSDToCurrency = 1.27M };
            Currency["MXN"] = new CurrencyConversion() { CountryCode = "MXN", CurrencyName = "Mexico Pesos", RateFromUSDToCurrency = 20.56M };
            Currency["CRC"] = new CurrencyConversion() { CountryCode = "CRC", CurrencyName = "Costa Rica Colons", RateFromUSDToCurrency = 642.83M };
            Currency["DZD"] = new CurrencyConversion() { CountryCode = "DZD", CurrencyName = "Algeria Dinars", RateFromUSDToCurrency = 140.26M };
            Currency["CNY"] = new CurrencyConversion() { CountryCode = "CNY", CurrencyName = "China Renminbis", RateFromUSDToCurrency = 6.35M };
            Currency["DKK"] = new CurrencyConversion() { CountryCode = "DKK", CurrencyName = "Denmark Krones", RateFromUSDToCurrency = 6.52M };
            Currency["PLN"] = new CurrencyConversion() { CountryCode = "PLN", CurrencyName = "Poland Zlotys", RateFromUSDToCurrency = 3.95M };
            return Currency;
        }
    }
}
