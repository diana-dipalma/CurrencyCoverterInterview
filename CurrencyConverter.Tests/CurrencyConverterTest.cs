using NUnit.Framework;
using CurrencyConverter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            Assert.AreEqual(1.00M, _currencyConverter.GetConvertedAmount("USD", "USD", 1M));
            Assert.AreEqual(20.56M, _currencyConverter.GetConvertedAmount("USD", "MXN", 1M));
            Assert.AreEqual(6.35M, _currencyConverter.GetConvertedAmount("USD", "CAD", 5M));
            Assert.AreEqual(0.62M, _currencyConverter.GetConvertedAmount("MXN", "CAD", 10M));
            Assert.AreEqual(0.61M, _currencyConverter.GetConvertedAmount("DKK", "PLN", 1M));

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
                    new CurrencyConversion()
                    {
                        CurrencyCode = "USD",
                        CurrencyName = "United States Dollars",
                        RateFromUSDToCurrency = 1M
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
            Assert.That(ex.Message, Is.EqualTo("Invalid rate for conversion from USD to CAD: 0"));
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
                    new CurrencyConversion()
                    {
                        CurrencyCode = "USD",
                        CurrencyName = "United States Dollars",
                        RateFromUSDToCurrency = 1M
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
            Assert.That(ex.Message, Is.EqualTo("Invalid rate for conversion from USD to JPY: -138.62"));
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

        private class BadCurrencyCodeRepository4 : ICurrencyConverterRepository
        {
            public IEnumerable<CurrencyConversion> GetConversions()
            {
                return new[]
                {
                    new CurrencyConversion()
                    {
                        CurrencyName = "code was null",
                        RateFromUSDToCurrency = 1.0M
                    },
                };
            }
        }

        [Test]
        public void TestBadCurrencyCodeRepository4()
        {
            var bad_repository = new BadCurrencyCodeRepository4();
            var ex = Assert.Throws<System.ArgumentException>(
                () => new CurrencyConverter(bad_repository)
            );
            Assert.That(
                ex.Message,
                Is.EqualTo("Currency codes must be 3 uppercase alpha characters: ")
            );
        }

        [Test]
        public void TestGetCurrencyName()
        {
            Assert.That(
                _currencyConverter.GetCurrencyName("USD"),
                Is.EqualTo("United States Dollars")
            );
        }

        [Test]
        public void TestUnsupportedIntermediateCurrency()
        {
            var ex = Assert.Throws<System.ArgumentException>(
                () =>
                    _currencyConverter.GetConvertedAmount(
                        "CAD",
                        "MXN",
                        new string[] { "USD", "EUR" },
                        1
                    )
            );
            Assert.That(ex.Message, Is.EqualTo("Unsupported intermediate currency: EUR"));
        }

        [Test]
        public void TestMissingIntermediateCurrency()
        {
            var ex = Assert.Throws<System.ArgumentException>(
                () =>
                    _currencyConverter.GetConvertedAmount(
                        "CAD",
                        "MXN",
                        new string[] { "DKK", "PLN" },
                        1
                    )
            );
            Assert.That(
                ex.Message,
                Is.EqualTo("No conversion rate found for CAD to MXN via any of [DKK, PLN]")
            );
        }

        [Test]
        public void TestConstructUpdateFromLegacyConversions()
        {
            var update = new CurrencyUpdate(
                new CurrencyConverterRepository().GetConversions());
            Assert.AreEqual(
                update,
                new CurrencyUpdate(
                    new string[0],
                    new CurrencyInfo[] {
                        new CurrencyInfo("USD", "United States Dollars", 2, null),
                        new CurrencyInfo("CAD", "Canada Dollars", 2, null),
                        new CurrencyInfo("MXN", "Mexico Pesos", 2, null),
                        new CurrencyInfo("CRC", "Costa Rica Colons", 2, null),
                        new CurrencyInfo("DZD", "Algeria Dinars", 2, null),
                        new CurrencyInfo("CNY", "China Renminbis", 2, null),
                        new CurrencyInfo("DKK", "Denmark Krones", 2, null),
                        new CurrencyInfo("PLN", "Poland Zlotys", 2, null),
                    },
                    new CurrencyConversionPair[] {
                        new CurrencyConversionPair("USD", "USD", 1.0M),
                        new CurrencyConversionPair("USD", "CAD", 1.27M),
                        new CurrencyConversionPair("USD", "MXN", 20.56M),
                        new CurrencyConversionPair("USD", "CRC", 642.83M),
                        new CurrencyConversionPair("USD", "DZD", 140.26M),
                        new CurrencyConversionPair("USD", "CNY", 6.35M),
                        new CurrencyConversionPair("USD", "DKK", 6.52M),
                        new CurrencyConversionPair("USD", "PLN", 3.95M),
                    }
                )
            );
        }

        [Test]
        public void TestDeleteCurrency()
        {
            Assert.AreEqual(6.35M, _currencyConverter.GetConvertedAmount("USD", "CAD", 5.00M));
            Assert.AreEqual(5.00M, _currencyConverter.GetConvertedAmount("CAD", "USD", 6.35M));
            Assert.AreEqual(0.62M, _currencyConverter.GetConvertedAmount("MXN", "CAD", 10.00M));
            // Weird rounding but it checks out: 0.62*20.56/1.27 = 10.037165
            Assert.AreEqual(10.04M, _currencyConverter.GetConvertedAmount("CAD", "MXN", 0.62M));
            Assert.AreEqual("Canada Dollars", _currencyConverter.GetCurrencyName("CAD"));

            var updateWithDeletion = new CurrencyUpdate(
               new string[] { "CAD" }, null, null);
            _currencyConverter.ProcessUpdate(updateWithDeletion);

            var ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.GetConvertedAmount("USD", "CAD", 5.00M)
            );
            Assert.That(ex.Message, Is.EqualTo("Unsupported currency: toCurrency=CAD"));

            ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.GetConvertedAmount("CAD", "USD", 6.35M)
            );
            Assert.That(ex.Message, Is.EqualTo("Unsupported currency: fromCurrency=CAD"));

            ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.GetConvertedAmount("MXN", "USD", new string[] { "CAD" }, 1M)
            );
            Assert.That(ex.Message, Is.EqualTo("Unsupported intermediate currency: CAD"));
        }

        [Test]
        public void TestUpdateInfoAndRounding()
        {
            Assert.AreEqual(10.04M, _currencyConverter.GetConvertedAmount("CAD", "MXN", 0.62M));
            Assert.AreEqual("Mexico Pesos", _currencyConverter.GetCurrencyName("MXN"));

            // Change name and set rounding to 0 decimal places.
            var updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo("MXN", "Mexican Pesos", 0, null),
               },
               null);
            _currencyConverter.ProcessUpdate(updateWithInfo);
            Assert.AreEqual(10M, _currencyConverter.GetConvertedAmount("CAD", "MXN", 0.62M));
            Assert.AreEqual("Mexican Pesos", _currencyConverter.GetCurrencyName("MXN"));

            // Change name back and set rounding to 3 decimal places
            updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo("MXN", "Mexico Pesos", 3, null),
               },
               null);
            _currencyConverter.ProcessUpdate(updateWithInfo);
            Assert.AreEqual(10.037M, _currencyConverter.GetConvertedAmount("CAD", "MXN", 0.62M));
            Assert.AreEqual("Mexico Pesos", _currencyConverter.GetCurrencyName("MXN"));

            // Set rounding to nearest 5 cents
            updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo("MXN", "Mexico Pesos", null, 0.05M),
               },
               null);
            _currencyConverter.ProcessUpdate(updateWithInfo);
            Assert.AreEqual(10.05M, _currencyConverter.GetConvertedAmount("CAD", "MXN", 0.62M));

            updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo(null, "Mexico Pesos", null, 0.05M),
               },
               null);
            var ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.ProcessUpdate(updateWithInfo));
            Assert.That(
                ex.Message,
                Is.EqualTo("Currency codes must be 3 uppercase alpha characters: "));

            updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo("MXN", null, null, 0.05M),
               },
               null);
            ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.ProcessUpdate(updateWithInfo));
            Assert.That(
                ex.Message,
                Is.EqualTo("Invalid name for currency MXN: "));

            updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo("MXN", "Mexico Pesos", null, null),
               },
               null);
            ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.ProcessUpdate(updateWithInfo));
            Assert.That(
                ex.Message,
                Is.EqualTo("Exactly one of decimalDigits and roundingIncrement must be " +
                "set in currency info for MXN: decimalDigits=, roundingIncrement="));

            updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo("MXN", "Mexico Pesos", 2, 0.05M),
               },
               null);
            ex = Assert.Throws<System.ArgumentException>(
                () => _currencyConverter.ProcessUpdate(updateWithInfo));
            Assert.That(
                ex.Message,
                Is.EqualTo("Exactly one of decimalDigits and roundingIncrement must be " +
                "set in currency info for MXN: decimalDigits=2, roundingIncrement=0.05"));

            // Test adding info for a new currency rather than updating an existing one.
            updateWithInfo = new CurrencyUpdate(
               null,
               new CurrencyInfo[] {
                   new CurrencyInfo("EUR", "Euros", 2, null),
               },
               null);
            _currencyConverter.ProcessUpdate(updateWithInfo);
            Assert.AreEqual("Euros", _currencyConverter.GetCurrencyName("EUR"));
        }

        [Test]
        public void TestUpdateRatesAndConvertThroughMXN()
        {
            // Rates are through USD using default legacy repository
            Assert.AreEqual(31.98M, _currencyConverter.GetConvertedAmount("CRC", "MXN", 1000M));
            Assert.AreEqual(61.77M, _currencyConverter.GetConvertedAmount("MXN", "CAD", 1000M));
            Assert.AreEqual(1.98M, _currencyConverter.GetConvertedAmount("CRC", "CAD", 1000M));

            // Rates as of 2023-05-24
            var updateWithNewRates = new CurrencyUpdate(
               null,
               null,
               new CurrencyConversionPair[] {
                   new CurrencyConversionPair("CRC", "MXN", 0.033M),
                   new CurrencyConversionPair("MXN", "CAD", 0.076M),
               });
            _currencyConverter.ProcessUpdate(updateWithNewRates);

            // Using new direct rates
            Assert.AreEqual(33.00M, _currencyConverter.GetConvertedAmount("CRC", "MXN", 1000M));
            Assert.AreEqual(76.00M, _currencyConverter.GetConvertedAmount("MXN", "CAD", 1000M));

            // Can convert through MXN. 1000 CRC = 33 MXN, 33 MXN = 2.51 CAD
            Assert.AreEqual(2.51M, _currencyConverter.GetConvertedAmount(
                "CRC", "CAD", new string[] { "MXN" }, 1000M));
        }

        [Test]
        public void TestStressTestConcurrentCalls()
        {
            Parallel.For(0, 1000, i =>
            {
                var random = new Random();
                // This will set USD to USD etc to things besides 1 but we don't care,
                // about the rates or amounts, just making sure this avoids things like this:
                // Failed TestStressTestConcurrentCalls [18 ms]
                // Error Message:
                //  System.AggregateException : One or more errors occurred. (Collection was modified after the enumerator was instantiated.) (Collection was modified after the enumerator was instantiated.)
                // ----> System.InvalidOperationException : Collection was modified after the enumerator was instantiated.
                // ----> System.InvalidOperationException : Collection was modified after the enumerator was instantiated.
                var currencies = new string[] {"USD", "CAD", "MXN", "CRC"};
                var fromCurrency = currencies[random.Next(currencies.Length)];
                var toCurrency = currencies[random.Next(currencies.Length)];
                // Cycling every fourth iteration: change rates, convert amounts, change names, look up names.
                switch (i % 4)
                {
                case 0:
                    var rate = random.Next(1, 1001) * 0.01M;
                    var updateWithNewRate = new CurrencyUpdate(
                       null,
                       null,
                       new CurrencyConversionPair[] {
                           new CurrencyConversionPair(fromCurrency, toCurrency, rate),
                       });
                    _currencyConverter.ProcessUpdate(updateWithNewRate);
                    break;
                case 1:
                    _currencyConverter.GetConvertedAmount(fromCurrency, toCurrency, 1000M);
                    break;
                case 2:
                    int currencyIndex = random.Next(currencies.Length);
                    var name = currencies[currencyIndex] + i;
                    var updateWithNewName = new CurrencyUpdate(
                       null,
                       new CurrencyInfo[] {
                           new CurrencyInfo(currencies[currencyIndex], name, 2, null),
                       },
                       null);
                       break;
                case 3:
                    _currencyConverter.GetCurrencyName(fromCurrency);
                    break;
                }
            });
        }
    }
}
