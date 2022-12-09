using NUnit.Framework;
using CurrencyConverter;
using System;

namespace CurrencyConverter.Tests
{
    public class CurrencyConverterTest
    {
        private CurrencyConverter currencyConverter;

        [SetUp]
        public void Setup()
        {
            currencyConverter  = new CurrencyConverter();
        }

        [Test]
        public void TestConversions()
        {
            Assert.AreEqual(20.56, currencyConverter.GetConvertedAmount("USD", "MXN", 1));
            Assert.AreEqual(6.35, currencyConverter.GetConvertedAmount("USD", "CAD", 5));
            Assert.AreEqual(0.62, currencyConverter.GetConvertedAmount("MXN", "CAD", 10));
            Assert.AreEqual(0.61, currencyConverter.GetConvertedAmount("DKK", "PLN", 1));
        }

        [Test]
        public void TestNotFoundCountry()
        {
            Assert.AreEqual(-1, currencyConverter.GetConvertedAmount("POR", "USD", 10));
            Assert.AreEqual(-1, currencyConverter.GetConvertedAmount("USD", "POR", 10));
            Assert.AreEqual(-1, currencyConverter.GetConvertedAmount("POR", "POR", 10));
        }

        [Test]
        public void TestForNegativeVal()
        {
            Assert.AreEqual(-6.35, currencyConverter.GetConvertedAmount("USD", "CAD", -5));
        }

        [Test]
        public void TestForSameOrginalCountryAndSameConversionCountry()
        {
            Assert.AreEqual(45, currencyConverter.GetConvertedAmount("MXN", "MXN", 45));
        }


        [Test] 
        public void TestConvertToUSACurrency()
        {
            Assert.AreEqual(0.05, currencyConverter.GetConvertedAmount("MXN", "USD", 1));
        }


        [Test]
        public void TestWithZeroValue()
        {
            Assert.AreEqual(0, currencyConverter.GetConvertedAmount("DKK", "PLN", 0));
        }
    }
}