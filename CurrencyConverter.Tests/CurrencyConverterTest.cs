using NUnit.Framework;
using CurrencyConverter;
using System;

namespace CurrencyConverter.Tests
{
    public class CurrencyConverterTest
    {
        private CurrencyConverter currencyConverter = new CurrencyConverter();

        [SetUp]
        public void Setup()
        {
            //Assert.Fail("NOT IMPLEMENTED");
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
        public void TestUSDToEveryCurrency()
        {
            //Test Every Amount 1 dollar
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("USD", "USD", 1));
            Assert.AreEqual(1.27, currencyConverter.GetConvertedAmount("USD", "CAD", 1));
            Assert.AreEqual(20.56, currencyConverter.GetConvertedAmount("USD", "MXN", 1));
            Assert.AreEqual(642.83, currencyConverter.GetConvertedAmount("USD", "CRC", 1));
            Assert.AreEqual(140.26, currencyConverter.GetConvertedAmount("USD", "DZD", 1));
            Assert.AreEqual(6.35, currencyConverter.GetConvertedAmount("USD", "CNY", 1));
            Assert.AreEqual(6.52, currencyConverter.GetConvertedAmount("USD", "DKK", 1));
            Assert.AreEqual(3.95, currencyConverter.GetConvertedAmount("USD", "PLN", 1));
        }

        [Test]
        public void TestEveryCurrencyToUSD()
        {
            //Test Every Amount 1 dollar
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("USD", "USD", 1));
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("CAD", "USD", 1.27M));
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("MXN", "USD", 20.56M));
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("CRC", "USD", 642.83M));
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("DZD", "USD", 140.26M));
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("CNY", "USD", 6.35M));
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("DKK", "USD", 6.52M));
            Assert.AreEqual(1, currencyConverter.GetConvertedAmount("PLN", "USD", 3.95M));
        }
        [Test]
        public void TestZeroValues()
        { //Test 0
            Assert.AreEqual(0, currencyConverter.GetConvertedAmount("USD", "PLN", 0));
            Assert.AreEqual(0, currencyConverter.GetConvertedAmount("USD", "USD", 0));
            Assert.AreEqual(0, currencyConverter.GetConvertedAmount("PLN", "USD", 0));
        }
        [Test]
        public void TestBigValues()
        { //Test Large Values
            Assert.AreEqual(39500000000, currencyConverter.GetConvertedAmount("USD", "PLN", 10000000000));
            Assert.AreEqual(123456789, currencyConverter.GetConvertedAmount("USD", "USD", 123456789));
            Assert.AreEqual(31170410917.22, currencyConverter.GetConvertedAmount("PLN", "USD", 123123123123));
        }
        [Test]
        public void TestNegative()
        { //Test Large Values
            Assert.AreEqual(-3.95, currencyConverter.GetConvertedAmount("USD", "PLN", -1));
            Assert.AreEqual(-1000, currencyConverter.GetConvertedAmount("USD", "USD", -1000));
            Assert.AreEqual(-25316455.7, currencyConverter.GetConvertedAmount("PLN", "USD", -100000000));
        }
    }
}