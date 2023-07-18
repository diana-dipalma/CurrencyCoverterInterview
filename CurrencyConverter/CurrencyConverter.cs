using System;
using System.Collections.Generic;

namespace CurrencyConverter
{
    public class RateNotFoundException : ArgumentException
    {
        public RateNotFoundException(string message) : base(message)
        {
        }
    }
    public class CurrencyConverter
    {
        private Dictionary<string, CurrencyConversion> _rates;

        public CurrencyConverter()
        { 
            var repo = new CurrencyConverterRepository();

            _rates = new Dictionary<string, CurrencyConversion>();

            foreach (var conversion in repo.GetConversions())
            {
                if (conversion.RateFromUSDToCurrency <= 0m)
                {
                    // Checking for 0 lets us avoid thinking about division by 0 later,
                    // and negative rates don't make sense either.
                    throw new ArgumentException($"Nonsensical conversion rate for {conversion.CurrencyCode}");
                }
                // We could precompute and store all currency pairs here, but it's also fast to calculate at run time.
                // (In the airline pricing world, IATA publishes ICER rates for currency pairs. (And then there are
                // multiple rates: BSR for pricing, ICH or ROE for other parts.. and every currency has its
                // own rounding and decimal rules. Glad we don't have to do all that here :)
                // We are just triangulating through USD, the Neutral Unit of Currency
                _rates.Add(conversion.CurrencyCode, conversion);
            }
            // assert that _rates["USD"] exists?
        }
        public CurrencyConversion GetConversion(string currencyCode)
        {
            CurrencyConversion conversion;
            if (!_rates.TryGetValue(currencyCode, out conversion))
            {
                throw new RateNotFoundException($"Unknown currency: {currencyCode}");
            }
            return conversion;
        }
       
        // TODO: does C# use docstrings or such? E.g. to say which exceptions this may raise.
        public Decimal GetConvertedAmount(string fromCurrency, string toCurrency, decimal amount)
        {
            CurrencyConversion fromConv, toConv;

            // Even if we don't know about a currency, we can still return something for these
            // special cases. Although maybe we should throw an exception instead for consistency.
            // (I.e. move the assignment to fromConv and toConv to the top).
            if (fromCurrency == toCurrency)
            {
                // no math needed
                return amount;
            }
            if (amount == 0m)
            {
                return amount;
            }
            
            fromConv = GetConversion(fromCurrency.ToUpper());
            toConv = GetConversion(toCurrency.ToUpper());

            // It's great that C# has a 20+ digit precision "Decimals" type - we don't have to
            // worry about float precision issues, or make our own type :o
            Decimal result;

            // Technically, no need to special case USD since it's in the repo with 1.0 rate.
            if (fromConv.CurrencyCode == "USD")
            {
                result = amount * toConv.RateFromUSDToCurrency;
            }
            else if (toConv.CurrencyCode == "USD")
            {
                result = amount / fromConv.RateFromUSDToCurrency;
            }
            else
            {
                result = amount * toConv.RateFromUSDToCurrency / fromConv.RateFromUSDToCurrency;
            }
            // I guess we round every currency to 2 decimals.
            // Do that in the way that bankers do, according to the internet (see TestMidpointRounding).
            return Math.Round(result, 2, MidpointRounding.ToEven);
        }
    }
}
