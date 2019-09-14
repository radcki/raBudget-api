using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using raBudget.Domain.Enum;

namespace raBudget.Domain.Entities
{
    public class Currency
    {
        public eCurrency CurrencyCode { get; private set; }
        public string Code { get; private set; }
        public NumberFormatInfo NumberFormat { get; private set; }
        public string Symbol { get; private set; }
        public string EnglishName { get; private set; }
        public string NativeName { get; private set; }

        private Currency(){}

        /// <summary>
        /// Constructs a currency object with a NumberFormatInfo.
        /// </summary>
        /// <param name="currencyCode"></param>
        public Currency(eCurrency currencyCode)
        {
            CurrencyCode = currencyCode;
            Code = System.Enum.GetName(typeof(eCurrency), CurrencyCode);
            var cultureInfo = CultureInfoFromCurrencyISO(Code);
            NumberFormat = cultureInfo.NumberFormat;
            var region = new RegionInfo(cultureInfo.LCID);
            Symbol = region.CurrencySymbol;
            EnglishName = region.CurrencyEnglishName;
            NativeName = region.CurrencyNativeName;
        }

        public static Currency Get(eCurrency currencyCode)
        {
            if (CurrencyDictionary.ContainsKey(currencyCode))
                return CurrencyDictionary[currencyCode];
            else
                return null;
        }

        public static bool Exists(eCurrency currencyCode)
        {
            return CurrencyDictionary.ContainsKey(currencyCode);
        }

        private static CultureInfo CultureInfoFromCurrencyISO(string isoCode)
        {
            //CultureInfo cultureInfo = (from culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            //  let region = new RegionInfo(culture.LCID)
            //  where String.Equals(region.ISOCurrencySymbol, isoCode, StringComparison.InvariantCultureIgnoreCase)
            //  select culture).First();
            //return cultureInfo;
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo ri = new RegionInfo(ci.LCID);
                if (ri.ISOCurrencySymbol == isoCode)
                    return ci;
            }
            throw new Exception("Currency code " + isoCode + " is not supported by the current .Net Framework.");
        }

        private static Dictionary<eCurrency, Currency> _currencyDictionary;
        private static Dictionary<eCurrency, Currency> CurrencyDictionary
        {
            get { return _currencyDictionary ?? (_currencyDictionary = CreateCurrencyDictionary()); }
        }
        private static Dictionary<eCurrency, Currency> CreateCurrencyDictionary()
        {
            var result = new Dictionary<eCurrency, Currency>();
            foreach (eCurrency code in System.Enum.GetValues(typeof(eCurrency)))
                result.Add(code, new Currency(code));
            return result;
        }
    }
}
