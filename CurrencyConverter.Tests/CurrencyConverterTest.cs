using NUnit.Framework;
using CurrencyConverter;

namespace CurrencyConverter.Tests
{
    public class CurrencyConverterTest
    {
        private CurrencyConverter _currencyConverter;

        [SetUp]
        public void Setup()
        {
            _currencyConverter = new CurrencyConverter();
        }

        [Test]
        public void TestConversions()
        {
            Assert.AreEqual(20.56, _currencyConverter.GetConvertedAmount("USD", "MXN", 1));
            Assert.AreEqual(6.35, _currencyConverter.GetConvertedAmount("USD", "CAD", 5));
            Assert.AreEqual(0.62, _currencyConverter.GetConvertedAmount("MXN", "CAD", 10));
            Assert.AreEqual(0.61, _currencyConverter.GetConvertedAmount("DKK", "PLN", 1));
        }
    }
}