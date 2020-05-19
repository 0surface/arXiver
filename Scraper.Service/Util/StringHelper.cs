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
    }
}
