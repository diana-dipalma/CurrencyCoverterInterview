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

        [Test]
        public void TestSameCurrency()
        {
            Assert.AreEqual(1.1m, currencyConverter.GetConvertedAmount("USD", "USD", 1.1m));
            // This currency code is not even in the repo
            Assert.AreEqual(5m, currencyConverter.GetConvertedAmount("ELK", "ELK", 5m));
        }

        [Test]
        public void TestUnknownCurrency()
        {
            Assert.Throws<RateNotFoundException>(() => currencyConverter.GetConvertedAmount("USD", "ELK", 1.1m));
            Assert.Throws<RateNotFoundException>(() => currencyConverter.GetConvertedAmount("ELK", "USD", 1.1m));
        }
    }
}