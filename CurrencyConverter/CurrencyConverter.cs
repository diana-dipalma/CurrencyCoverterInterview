using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyConverter
{
    public class CurrencyConverter
    {
        private readonly CurrencyConverterRepository _repository;

        public CurrencyConverter()
        {
            _repository = new CurrencyConverterRepository();
        }

        public decimal GetConvertedAmount(string fromCurrencyCode, string toCurrencyCode, decimal amount)
{
    try
    {
        // Return 0 if either the "from" or "to" currency code is null or empty
        if (string.IsNullOrEmpty(fromCurrencyCode) || string.IsNullOrEmpty(toCurrencyCode)) return 0;

        // Get the conversion rates
        var conversions = _repository.GetConversions();

        // Find the conversion rates for the given currencies
        var fromConversion = conversions.FirstOrDefault(c => c.CountryCode == fromCurrencyCode);
        var toConversion = conversions.FirstOrDefault(c => c.CountryCode == toCurrencyCode);

        // Return 0 if either of the conversions was not found
        if (fromConversion == null || toConversion == null) return 0;

        // Case 1: From USD to other currency
        if (fromCurrencyCode == "USD" && toCurrencyCode != "USD")
        {
            return amount * toConversion.RateFromUSDToCurrency;
        }
        // Case 2: From other currency to USD
        else if (fromCurrencyCode != "USD" && toCurrencyCode == "USD")
        {
            return amount * (1 / fromConversion.RateFromUSDToCurrency);
        }
        // Case 3: From other currency to another currency
        else
        {
            // Convert the "from" currency to USD
            var convertedToUSD = amount * (1 / fromConversion.RateFromUSDToCurrency);

            // Use case 2 to convert from USD to the "to" currency
            return convertedToUSD * toConversion.RateFromUSDToCurrency
;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while converting the currency: {0}", ex.Message);
                throw;
            }
        }
    }
}
