using NUnit.Framework;
using System.Collections.Generic;

namespace CurrencyConverter.Tests
{
    public class BadTestRepository : CurrencyConverterRepository
    {
        public override IEnumerable<CurrencyConversion> GetConversions()
        {
            // 0 or negative rates are not allowed
            return new[] {
                new CurrencyConversion() { CurrencyCode = "USD", CurrencyName = "United States Dollars", RateFromUSDToCurrency = 1.0M },
                new CurrencyConversion() { CurrencyCode = "CAD", CurrencyName = "Canada Dollars", RateFromUSDToCurrency = 0M }
            };
        }
    }

    public class CurrencyConverterTest
    {
        private CurrencyConverter currencyConverter;

        [SetUp]
        public void Setup()
        {
            // In a real system, we would have a separate repo with fixed test rates,
            // vs loading the real live rates, because those will change and then our
            // tests break / no longer test what they were meant to.
            var repo = new CurrencyConverterRepository();
            currencyConverter = new CurrencyConverter(repo);
        }

        [Test]
        public void TestConversions()
        {
            Assert.AreEqual(20.56, currencyConverter.GetConvertedAmount("USD", "MXN", 1));
            Assert.AreEqual(6.35, currencyConverter.GetConvertedAmount("USD", "CAD", 5));
            // .6177.. rounds to .62
            Assert.AreEqual(0.62, currencyConverter.GetConvertedAmount("MXN", "CAD", 10));
            // .6058.. rounds to .61
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

        [Test]
        public void TestVerySmallAmountRounding()
        {
            // The way I have coded it, 0.01 CRC (which is 10^-5 USD) ends up 0 USD.
            // Initially I had felt it should round to 1 US cent, but that would be strange too? 
            Assert.AreEqual(0.00m, currencyConverter.GetConvertedAmount("CRC", "USD", 0.01m));
            Assert.AreEqual(0.00m, currencyConverter.GetConvertedAmount("CRC", "CAD", 0.01m));
        }

        [Test]
        public void TestMidpointRounding()
        {
            // Another debatable decision. The rate for CNY is 6.35 so .10 USD is .635 CNY.
            // What should that round to? I picked "ToEven" because that will average things
            // out over a large number of conversions, and it's called "Banker's Rounding".
            // (At first I had "AwayFromZero" which would always round up positive amounts.)
            // So, 0.635 rounds up to 0.64
            Assert.AreEqual(.64m, currencyConverter.GetConvertedAmount("USD", "CNY", 0.10m));
            // ...but 1.905 rounds down to 1.90
            Assert.AreEqual(1.90m, currencyConverter.GetConvertedAmount("USD", "CNY", 0.30m));
        }

        [Test]
        public void TestAllowNegativeAmounts()
        {
            // Why not. Also, allow lower case.
            Assert.AreEqual(-.64m, currencyConverter.GetConvertedAmount("usd", "cny", -0.10m));
            Assert.AreEqual(-1.90m, currencyConverter.GetConvertedAmount("Usd", "CNY", -0.30m));
        }

        [Test]
        public void TestLargeAmounts()
        {
            // Maybe the converter should enforce an upper limit on amounts?
            // Right now you can pass in any valid Decimal; Decimal has about 28/9 digits of precision,
            // so this here (26 zeroes before the decimal point) is the largest where the 1 cent still
            // gets correctly preserved.
            Assert.AreEqual(127000000000000000000000000.01m,
                currencyConverter.GetConvertedAmount("USD", "CAD", 100000000000000000000000000.01m));
            Assert.AreEqual(100000000000000000000000000.01m,
                currencyConverter.GetConvertedAmount("CRC", "USD", 64283000000000000000000000006.43m));
        }

        [Test]
        public void TestNonsensicalRatesInRepo()
        {
            var repo = new BadTestRepository();
            // assert that constructing a CurrencyConverter with the bad repo throws an exception
            Assert.Throws<System.ArgumentException>(() => new CurrencyConverter(repo));
        }
    }
}