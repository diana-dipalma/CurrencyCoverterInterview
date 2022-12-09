using System;
using System.Runtime.CompilerServices;

namespace CurrencyConverter
{
    public class Converter
    {
        public static string inputTo;
        public static decimal amount;

        public static decimal GetConvertedAmount()
        {
            // go through cc repository array and find the country code that matches inputted code

            var countryData = new CurrencyConverterRepository(); 
            decimal rate = 0;

            foreach (CurrencyConversion country in countryData)
            // countryData is enumerable not enumerator so it cant be foreach'ed like this yet
            // I was checking out this in the docs https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.toarray?view=net-7.0
            {
                if (inputTo == country.CountryCode) rate = country.RateFromUSDToCurrency; 
                // I wanted to start by just working with USD and going from there to make it easier to work with
                    // would need to add code to search for both country codes - this could even possibly be its own method too
            }

            //return result of calculations
            Console.WriteLine($"The resulting amount is {amount * rate} USD");
            return amount * rate;
        }
        static void Main()
        {
            
            // get input from user
            // check if input is a number > 0, repository contains currency requested, etc
            // if invalid, throw errors
            // if valid, get the conversion rate and return the amount in USD
            
            Console.WriteLine("TO currency:");
            inputTo = Console.ReadLine();
            Console.WriteLine("Amount:");
            amount = Convert.ToDecimal(Console.ReadLine());
            
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Please enter a valid number");


        }
}
}
