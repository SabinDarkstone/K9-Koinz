using System.Globalization;

namespace K9_Koinz.Utils {
    public static class CurrencyUtils {
        public static string FormatCurrency(this double value, int decimalPlaces = 2) {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyDecimalDigits = decimalPlaces;
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.CurrencyGroupSeparator = ",";
            culture.NumberFormat.CurrencyNegativePattern = 1;
            return string.Format(culture, "{0:C}", value);
        }
    }
}
