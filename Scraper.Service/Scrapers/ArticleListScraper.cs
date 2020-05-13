using HtmlAgilityPack;
using Scraper.Service.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scraper.Service.Scrapers
{
    public interface IArticleListScraper
    {
		int GetArticleListEntryCount(HtmlDocument htmlDoc, string[] identifierStrings
			, string articleListCountSelector = "//small");
    }

    public class ArticleListScraper : IArticleListScraper
    {
		public ArticleListScraper()
		{
			//TODO : Inject repo
		}

		

		public int GetArticleListEntryCount(HtmlDocument htmlDoc, string[] identifierStrings
			, string articleListCountSelector = "//small")
		{
			int result = 0;

			try
			{
				//TODO: Persist these selectors in database	
				if (identifierStrings == null || identifierStrings.Length != 2)
				{
					identifierStrings = new string[] { "total of", "entries:" };
				}

				var docNode = htmlDoc.DocumentNode;

				var selectedNodeText = docNode.SelectNodes(articleListCountSelector)
											.Where(node => node.InnerText.Contains(identifierStrings[0])
													&& node.InnerText.Contains(identifierStrings[0]))
											?.FirstOrDefault()
											?.InnerText ?? string.Empty;

				if (!string.IsNullOrEmpty(selectedNodeText))
				{
					string splitText = selectedNodeText.Split(new string[] { identifierStrings[1] }
																, 3, StringSplitOptions.None)[0];

					int.TryParse(ScraperUtil.SafeRegexReplace(splitText, "[^0-9.]", ""), out result);
				}

				return result;
			}
			catch (Exception)
			{
				return -1;
			}
		}
	}
}
