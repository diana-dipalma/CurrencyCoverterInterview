using System;
using System.Collections.Generic;


namespace CurrencyConverter
{
    public class CurrencyConverter
    {
        /*
         * Input the code of orginal country, the code of coverted country and the amount of money
         * Return the conversion amount between two countries
         * Return -1 if the country code is not found in the list
         */

        public Decimal GetConvertedAmount(String OriginalCode, String ConversionCode, Decimal val)
        {
            //Get the list of conversion rate 
            CurrencyConverterRepository res = new CurrencyConverterRepository();
            IEnumerable<CurrencyConversion> list = res.GetConversions();

            Decimal USDRateForOrg = -1;
            Decimal USDRateForCon = -1;

            foreach(CurrencyConversion item in list)
            {
                //Search in the list to find the currency rate of orignal country vs USA
                if (item.CountryCode.Equals(OriginalCode))
                {
                    USDRateForOrg = item.RateFromUSDToCurrency;
                }

                //Search in the list to find the currency rate of conversion country vs USA
                if (item.CountryCode.Equals(ConversionCode))
                {
                    USDRateForCon = item.RateFromUSDToCurrency;
                }
            }

            //Not found the country in the list
            if(USDRateForOrg == -1 || USDRateForCon == -1)
            {
                return -1;
            }

            //Total conversion
            Decimal conversion = (val / USDRateForOrg) * USDRateForCon;

            return Math.Round(conversion,2);

        }
        
    }
}
