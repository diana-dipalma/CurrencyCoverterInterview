using NUnit.Framework;

namespace CurrencyConverter.Tests
{
    public class CurrencyConverterTest
    {
        private CurrencyConverter currencyConverter;

        [SetUp]
        public void Setup()
        {
            currencyConverter = new CurrencyConverter();
        }

        [Test]
        public void TestConversions()
        {
            Assert.AreEqual(20.56, currencyConverter.GetConvertedAmount("USD", "MXN", 1));
            Assert.AreEqual(6.36, currencyConverter.GetConvertedAmount("USD", "CAD", 5));
            Assert.AreEqual(0.62, currencyConverter.GetConvertedAmount("MXN", "CAD", 10));
            Assert.AreEqual(0.61, currencyConverter.GetConvertedAmount("DKK", "PLN", 1));
        }
    }
}