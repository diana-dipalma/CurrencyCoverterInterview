using NUnit.Framework;
using CurrencyConverter;
using System.Collections.Generic;

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
            Assert.AreEqual(1.00, _currencyConverter.GetConvertedAmount("USD", "USD", 1));
            Assert.AreEqual(20.56, _currencyConverter.GetConvertedAmount("USD", "MXN", 1));
            Assert.AreEqual(6.35, _currencyConverter.GetConvertedAmount("USD", "CAD", 5));
            Assert.AreEqual(0.62, _currencyConverter.GetConvertedAmount("MXN", "CAD", 10));
            Assert.AreEqual(0.61, _currencyConverter.GetConvertedAmount("DKK", "PLN", 1));

            var ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.GetConvertedAmount("EUR", "USD", 1)
            );
            Assert.That(ex.Message, Is.EqualTo("Unsupported currency: fromCurrency=EUR"));

            ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.GetConvertedAmount("USD", "EUR", 1)
            );
            Assert.That(ex.Message, Is.EqualTo("Unsupported currency: toCurrency=EUR"));
        }

        private class ZeroRateRepository : ICurrencyConverterRepository
        {
            public IEnumerable<CurrencyConversion> GetConversions()
            {
                return new[]
                {
                    new CurrencyConversion()
                    {
                        CurrencyCode = "CAD",
                        CurrencyName = "Canadian Dollars",
                        RateFromUSDToCurrency = 0M
                    },
                };
            }
        }

        [Test]
        public void TestZeroRateRepository()
        {
            var bad_repository = new ZeroRateRepository();
            var ex = Assert.Throws<System.ArgumentException>(
                () => new CurrencyConverter(bad_repository)
            );
            Assert.That(ex.Message, Is.EqualTo("Invalid rate for currency: CAD"));
        }

        private class NegativeRepository : ICurrencyConverterRepository
        {
            public IEnumerable<CurrencyConversion> GetConversions()
            {
                return new[]
                {
                    new CurrencyConversion()
                    {
                        CurrencyCode = "JPY",
                        CurrencyName = "Japanese Yen",
                        RateFromUSDToCurrency = -138.62M
                    },
                };
            }
        }

        [Test]
        public void TestNegativeRateRepository()
        {
            var bad_repository = new NegativeRepository();
            var ex = Assert.Throws<System.ArgumentException>(
                () => new CurrencyConverter(bad_repository)
            );
            Assert.That(ex.Message, Is.EqualTo("Invalid rate for currency: JPY"));
        }

        private class BadCurrencyCodeRepository1 : ICurrencyConverterRepository
        {
            public IEnumerable<CurrencyConversion> GetConversions()
            {
                return new[]
                {
                    new CurrencyConversion()
                    {
                        CurrencyCode = "UUSD",
                        CurrencyName = "United United States Dollars",
                        RateFromUSDToCurrency = 1M
                    },
                };
            }
        }

        [Test]
        public void TestBadCurrencyCodeRepository1()
        {
            var bad_repository = new BadCurrencyCodeRepository1();
            var ex = Assert.Throws<System.ArgumentException>(
                () => new CurrencyConverter(bad_repository)
            );
            Assert.That(
                ex.Message,
                Is.EqualTo("Currency codes must be 3 uppercase alpha characters: UUSD")
            );
        }

        private class BadCurrencyCodeRepository2 : ICurrencyConverterRepository
        {
            public IEnumerable<CurrencyConversion> GetConversions()
            {
                return new[]
                {
                    new CurrencyConversion()
                    {
                        CurrencyCode = "123",
                        CurrencyName = "United States Dollars",
                        RateFromUSDToCurrency = 123M
                    },
                };
            }
        }

        [Test]
        public void TestBadCurrencyCodeRepository2()
        {
            var bad_repository = new BadCurrencyCodeRepository2();
            var ex = Assert.Throws<System.ArgumentException>(
                () => new CurrencyConverter(bad_repository)
            );
            Assert.That(
                ex.Message,
                Is.EqualTo("Currency codes must be 3 uppercase alpha characters: 123")
            );
        }

        private class BadCurrencyCodeRepository3 : ICurrencyConverterRepository
        {
            public IEnumerable<CurrencyConversion> GetConversions()
            {
                return new[]
                {
                    new CurrencyConversion()
                    {
                        CurrencyCode = "uSD",
                        CurrencyName = "united States Dollars",
                        RateFromUSDToCurrency = 1.0M
                    },
                };
            }
        }

        [Test]
        public void TestBadCurrencyCodeRepository3()
        {
            var bad_repository = new BadCurrencyCodeRepository3();
            var ex = Assert.Throws<System.ArgumentException>(
                () => new CurrencyConverter(bad_repository)
            );
            Assert.That(
                ex.Message,
                Is.EqualTo("Currency codes must be 3 uppercase alpha characters: uSD")
            );
        }
    }
}
