using HtmlAgilityPack;
using System;
using System.Collections.Generic;
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

        public static List<HtmlNode> GetChildNodesByName (this HtmlNode node, string name)
        {
            if(node !=null && node.HasChildNodes)
            {
                return node.ChildNodes.Where(x => x.Name == name).ToList();
            }
            return null;
        }

        public static bool AttributeExists(this HtmlAttributeCollection collection, string name, string value)
        {
            if (collection == null) return false;

            var attr = collection.AttributesWithName(name)?.FirstOrDefault() ?? null;

            return (attr != null && attr.Value == value);
        }

        /// <summary>
        ///  Returns the inner text value of the htmlNode   
        /// </summary>
        /// <param name="htmlNode">HtmlNode</param>
        /// <param name="xpath">string </param>
        /// <returns>string</returns>
        public static string TryParseInnerText(this HtmlNode htmlNode, string xpath)
        {
            try
            {
                if (htmlNode != null)
                {
                    return htmlNode.SelectSingleNode(xpath).InnerText ?? string.Empty;
                }
            }
            catch (Exception) { }

            return string.Empty;
        }

        public static string TextfromOneNode(this HtmlNode node, string xmlPath)
        {
            string toReturn = "";
            var navigator = (HtmlAgilityPack.HtmlNodeNavigator)node.CreateNavigator();
            var result = navigator.SelectSingleNode(xmlPath);
            if (result != null)
            {
                toReturn = result.Value;
            }
            return toReturn;
        }

        
    }
}
