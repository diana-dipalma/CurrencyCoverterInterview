//using CurrencyConverter;
//using NUnit.Framework;
//using System;

//namespace CurrencyConverterTests
//{
//    public class CurrencyConverterTest
//    {
//        private CurrencyConverterTest currencyConverter;

//        [SetUp]
//        public void Setup()
//        {
//            Assert.Fail("NOT IMPLEMENTED");
//        }

//        [Test]
//        public void TestConversions()
//        {
//            var currencyConverter = new Converter();

//            Assert.AreEqual(20.56, currencyConverter.GetConvertedAmount("USD", "MXN", 1));
//            Assert.AreEqual(6.36, currencyConverter.GetConvertedAmount("USD", "CAD", 5));
//            Assert.AreEqual(0.62, currencyConverter.GetConvertedAmount("MXN", "CAD", 10));
//            Assert.AreEqual(0.61, currencyConverter.GetConvertedAmount("DKK", "PLN", 1));
//        }
//        [Test]
//        public void TestExceptions() 
//        {
//            Assert.Throws<ArgumentOutOfRangeException>;
//        }

//    }
//}

//commented out tests because I've never used C# before, so trying to learn the syntax, otherwise I would complete all test coverage first 
// need to add test coverage for errors/invalid input to be sure exceptions are thrown
