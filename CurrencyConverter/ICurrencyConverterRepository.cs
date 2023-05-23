using System.Collections.Generic;

namespace CurrencyConverter
{
    public interface ICurrencyConverterRepository
    {
        IEnumerable<CurrencyConversion> GetConversions();
    }
}