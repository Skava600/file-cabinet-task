using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Converters
{
    /// <summary>
    /// Input parameters converter.
    /// </summary>
    public static class InputConverter
    {
        /// <summary>
        /// Strings  converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// True if it is possible to convert, input string and resulting string.
        /// </returns>
        public static Tuple<bool, string, string> StringConverter(string input)
        {
            return new Tuple<bool, string, string>(true, input, input);
        }

        /// <summary>
        /// DateTime converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// True if it is possible to convert, input string and resulting DateTime.
        /// </returns>
        public static Tuple<bool, string, DateTime> DateTimeConverter(string input)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            bool result = DateTime.TryParse(input, culture, styles, out DateTime dateOfBirth);

            return new Tuple<bool, string, DateTime>(result, input, dateOfBirth);
        }

        /// <summary>
        /// char converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// True if it is possible to convert, input string and resulting char.
        /// </returns>
        public static Tuple<bool, string, char> CharConverter(string input)
        {
            bool result = char.TryParse(input, out char c);
            return new Tuple<bool, string, char>(result, input, c);
        }

        /// <summary>
        /// short converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// True if it is possible to convert, input string and resulting short.
        /// </returns>
        public static Tuple<bool, string, short> ShortConverter(string input)
        {
            bool result = short.TryParse(input, out short sh);
            return new Tuple<bool, string, short>(result, input, sh);
        }

        /// <summary>
        /// decimal converter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// True if it is possible to convert, input string and resulting decimal.
        /// </returns>
        public static Tuple<bool, string, decimal> DecimalConverter(string input)
        {
            bool result = decimal.TryParse(input, out decimal dec);
            return new Tuple<bool, string, decimal>(result, input, dec);
        }
    }
}
