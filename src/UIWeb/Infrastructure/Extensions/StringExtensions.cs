using System;
using System.Text;
using NExtensions;

namespace Wiz.Gringotts.UIWeb.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string TrimmedOrNull(this string item)
        {
            return string.IsNullOrWhiteSpace(item) ? null : item.Trim();
        }

        public static int? ToNullableInt32(this string item)
        {
            int i;
            if (int.TryParse(item, out i)) return i;
            return null;
        }

        public static decimal? ToNullableDecimal(this string item)
        {
            decimal i;
            if (decimal.TryParse(item, out i)) return i;
            return null;
        }

        public static string SSNMask(this string item)
        {
           return "XXX-XX-{0}".FormatWith(item.Substring(item.Length - 4, 4));
        }

        public static string ToCurrencyText(this string item)
        {
            decimal item1 = Convert.ToDecimal(item);
            // Round the value just in case the decimal value is longer than two digits always round up!
            item1 = decimal.Round(item1, 2, MidpointRounding.AwayFromZero);

            var wordNumber = string.Empty;

            // Divide the number into the whole and fractional part strings
            var arrNumber = item.ToString().Split('.');

            // Get the whole number text
            var wholePart = long.Parse(arrNumber[0]);
            var strWholePart = wholePart.NumberToText();

            // For amounts of zero dollars show 'No Dollars...' instead of 'Zero Dollars...'
            wordNumber = (wholePart == 0 ? "No" : strWholePart) + (wholePart == 1 ? " Dollar and " : " Dollars and ");

            // If the array has more than one element then there is a fractional part otherwise there isn't
            // just add 'No Cents' to the end
            if (arrNumber.Length > 1)
            {
                // If the length of the fractional element is only 1, append a 0

                var cents = (arrNumber[1].Length == 1 ? arrNumber[1] + "0" : arrNumber[1].Substring(0,2)) + "/100 ";

                wordNumber += cents;

            }
            else
                wordNumber += "00/100 ";

            return wordNumber + " *****";
        }


        public static string NumberToText(this long number)
        {
            var wordNumber = new StringBuilder();

            var powers = new[] { "Thousand ", "Million ", "Billion " };
            var tens = new[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            var ones = new[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
                                       "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

            if (number == 0) { return "Zero"; }
            if (number < 0)
            {
                wordNumber.Append("Negative ");
                number = -number;
            }

            var groupedNumber = new long[] { 0, 0, 0, 0 };
            var groupIndex = 0;

            while (number > 0)
            {
                groupedNumber[groupIndex++] = number % 1000;
                number /= 1000;
            }

            for (var i = 3; i >= 0; i--)
            {
                var group = groupedNumber[i];

                if (group >= 100)
                {
                    wordNumber.Append(ones[group / 100 - 1] + " Hundred ");
                    group %= 100;

                    if (group == 0 && i > 0)
                        wordNumber.Append(powers[i - 1]);
                }

                if (group >= 20)
                {
                    if ((group % 10) != 0)
                        wordNumber.Append(tens[group / 10 - 2] + " " + ones[group % 10 - 1] + " ");
                    else
                        wordNumber.Append(tens[group / 10 - 2] + " ");
                }
                else if (group > 0)
                    wordNumber.Append(ones[group - 1] + " ");

                if (group != 0 && i > 0)
                    wordNumber.Append(powers[i - 1]);
            }

            return wordNumber.ToString().Trim();
        }
    }
}