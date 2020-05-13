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

        /// <summary>
        /// Returns HtmlNode matching the passed node name
        /// </summary>
        /// <param name="htmlNode">HtmlAgilityPack HtmlNode object</param>
        /// <param name="currnetNodeName">string </param>
        /// <returns>HtmlNode object</returns>
        public static HtmlNode GetNextSibling(HtmlNode htmlNode, string currnetNodeName)
        {
            var currentNode = htmlNode;

            while(currentNode != null)
            {
                currentNode = currentNode.NextSibling;

                if (currentNode != null &&  currentNode.NodeType == HtmlNodeType.Element 
                    && currentNode.Name == currnetNodeName)
                    return currentNode;
            }

            return null;
        }

    }
}
