using System;
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
            // testing int amount
            Assert.AreEqual(20.56, currencyConverter.GetConvertedAmount("USD", "MXN", 1), .01);
            Assert.AreEqual(6.36, currencyConverter.GetConvertedAmount("USD", "CAD", 5), .02);  // this test is less accurate, so extra delta is needed
            Assert.AreEqual(0.62, currencyConverter.GetConvertedAmount("MXN", "CAD", 10), .01);
            Assert.AreEqual(0.61, currencyConverter.GetConvertedAmount("DKK", "PLN", 1), .01);

            // testing double amount
            Assert.AreEqual(0.915, currencyConverter.GetConvertedAmount("DKK", "PLN", 1.5), .01);
        }

        [Test]
        // testing invalid country input
        public void TestErrorConversions()
        {
            Assert.Throws<ArgumentException>(() => currencyConverter.GetConvertedAmount("USS", "MXN", 1));
        }

        [Test]
        // testing converting from and to the same country
        public void TestSameConversion()
        {
            Assert.AreEqual(5, currencyConverter.GetConvertedAmount("USD", "USD", 5), .01);
            Assert.AreEqual(5, currencyConverter.GetConvertedAmount("MXN", "MXN", 5), .01);
        }
    }
}