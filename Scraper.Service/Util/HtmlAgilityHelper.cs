using HtmlAgilityPack;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Service.Util
{
    public static class HtmlAgilityHelper
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

        /// <summary>
        /// Returns HtmlNode matching the passed node name
        /// </summary>
        /// <param name="htmlNode">HtmlAgilityPack HtmlNode object</param>
        /// <param name="currnetNodeName">string </param>
        /// <returns>HtmlNode object</returns>
        public static HtmlNode GetNextSibling(this HtmlNode htmlNode, string currnetNodeName)
        {
            var currentNode = htmlNode;

            while (currentNode != null)
            {
                currentNode = currentNode.NextSibling;

                if (currentNode != null && currentNode.NodeType == HtmlNodeType.Element
                    && currentNode.Name == currnetNodeName)
                    return currentNode;
            }

            return null;
        }

        public static HtmlNode GetPreviousSibling(this HtmlNode htmlNode, string nodeName)
        {
            var currentNode = htmlNode;

            while (currentNode != null)
            {
                currentNode = currentNode.PreviousSibling;

                if (currentNode != null && currentNode.NodeType == HtmlNodeType.Element
                    && currentNode.Name == nodeName)
                    return currentNode;
            }

            return null;
        }


        public static bool AttributeExists(this HtmlAttributeCollection collection, string name, string value)
        {
            if (collection == null) return false;

            var attr = collection.AttributesWithName(name)?.FirstOrDefault() ?? null;

            return (attr != null && attr.Value == value);
        }
    }
}
