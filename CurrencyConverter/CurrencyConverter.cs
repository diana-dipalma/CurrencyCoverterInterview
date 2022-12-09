using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CurrencyConverter
{
    public class CurrencyConverter
    {

        // finds the rate from the goal country to USD
        public double FindRate(IEnumerable<CurrencyConversion> conversions, string goalCountry)
        {
            foreach (CurrencyConversion conversion in conversions)
            {
                if (conversion.CountryCode == goalCountry)
                {
                    return (double)conversion.RateFromUSDToCurrency;
                }
            }
            throw new ArgumentException($"Invalid country code {goalCountry}");
        }

        // converts amount of currency in fromCountry currency to toCountry currency
        public double GetConvertedAmount(string fromCountry, string toCountry, double amount) {
            IEnumerable<CurrencyConversion> conversions = new CurrencyConverterRepository().GetConversions();

            double fromRate = FindRate(conversions, fromCountry);
            double toRate = FindRate(conversions, toCountry);

            return (toRate / fromRate) * amount;
        }
    }
}
