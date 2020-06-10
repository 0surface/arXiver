using System;
using System.Text.RegularExpressions;

namespace Scraper.Service.Util
{
    public static class StringHelper
    {
        public static string SafeRegexReplace(this string input, string pattern, string replacement)
        {
            try
            {
                return Regex.Replace(input, pattern, replacement);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string RegexFindInBetweenStrings(this string input, string first, string second)
        {
            if (string.IsNullOrEmpty(input)
                || (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(second)))
                return string.Empty;

            try
            {
                var arr1 = input.Split(first.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                return arr1.Length >= 2 ?
                     arr1[1].Split(second.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0].Trim()
                     : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
