using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Service.Util
{
    public static class ScraperUtil
    {
        public static async Task<HtmlDocument> GetHtmlDocument(string url, CancellationToken cancellationToken)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                return await web.LoadFromWebAsync(url, cancellationToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string SafeRegexReplace(string input, string pattern, string replacement)
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
