using System;
using System.Collections.Generic;

namespace CurrencyConverter
{
    public class CurrencyConverter
    {
        public Decimal GetConvertedAmount(String CountryCode1, String CountryCode2, decimal amount)
        {
            //Dictonary from Currency Converter Respository
            Dictionary<String, CurrencyConversion> Currency = (new CurrencyConverterRepository()).GetConversions();
            //All of the conversions are from USD so changed from USD
            decimal toUSD = (amount / Currency[CountryCode1].RateFromUSDToCurrency);
            //Convert from USD to new Amount
            decimal toNewCurrency = toUSD * Currency[CountryCode2].RateFromUSDToCurrency;
            return System.Math.Round(toNewCurrency,2);
        }

    }

}
