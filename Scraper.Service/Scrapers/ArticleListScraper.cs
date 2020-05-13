using HtmlAgilityPack;
using Scraper.Service.Util;
using Scraper.Types.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Service.Scrapers
{
	public interface IArticleListScraper
	{
		int GetArticleListEntryCount(HtmlDocument htmlDocument, string[] identifierStrings
			, string articleListCountSelector = "//small");

		List<ArticleScrapedDataDto> GetArticleList(HtmlDocument htmlDocument);

	}

	public class ArticleListScraper : IArticleListScraper
	{
		public ArticleListScraper()
		{
			//TODO : Inject repo
		}



		public int GetArticleListEntryCount(HtmlDocument htmlDocument, string[] identifierStrings
			, string articleListCountSelector = "//small")
		{
			int result = 0;

			try
			{
				//TODO: Persist these selectors in database	

				//Validate
				if (identifierStrings == null || identifierStrings.Length != 2)
				{
					identifierStrings = new string[] { "total of", "entries:" };
				}
				articleListCountSelector = string.IsNullOrEmpty(articleListCountSelector) ? "//small" : articleListCountSelector;

				//Extract data
				var docNode = htmlDocument.DocumentNode;
				var selectedNodeText = docNode.SelectNodes(articleListCountSelector)
											.Where(node => node.InnerText.Contains(identifierStrings[0])
													&& node.InnerText.Contains(identifierStrings[0]))
											?.FirstOrDefault()
											?.InnerText ?? string.Empty;

				//Process data
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

		private List<Tuple<string, HtmlNode>> GetArticlesByPublicationDate(HtmlDocument htmlDocument)
		{
			var parseMain = (from info in htmlDocument.DocumentNode.SelectNodes("//div[@id='dlpage']")
							 from h3Element in info.SelectNodes("h3")
							 let dlElement = ScraperUtil.GetNextSibling(h3Element, "dl")
							 where dlElement != null
							 select new
							 {
								 h3 = h3Element.InnerText,
								 dl = dlElement
							 }).ToList();

			List<Tuple<string, HtmlNode>> result = new List<Tuple<string, HtmlNode>>();

			foreach (var item in parseMain)
			{
				Tuple<string, HtmlNode> publicationDate =
					new Tuple<string, HtmlNode>(item.h3, item.dl);
				result.Add(publicationDate);
			}
			return result;
		}

		public List<ArticleScrapedDataDto> GetArticleList(HtmlDocument htmlDocument)
		{
			List<ArticleScrapedDataDto> result = new List<ArticleScrapedDataDto>();

			var dayAndArticlesList = GetArticlesByPublicationDate(htmlDocument);

			if (dayAndArticlesList == null || dayAndArticlesList.Count < 1)
				return result;

			foreach (var dayAndArticlesItem in dayAndArticlesList)
			{
				var dts = dayAndArticlesItem.Item2.ChildNodes.Where(c => c.Name == "dt");
				var dds = dayAndArticlesItem.Item2.ChildNodes.Where(c => c.Name == "dd");

				var parsedQ = dts.Zip(dds,
									(dtElement, ddElement) => new
									{
										pub_date = dayAndArticlesItem.Item1,

										AbstractAnchorText = dtElement.SelectSingleNode("//a[@title='Abstract']").InnerText,
										AbstractUrl = dtElement.SelectSingleNode("//a[@title='Abstract']").GetAttributeValue("href", ""),
										pdfUrl = dtElement.SelectSingleNode("//a[@title='Download PDF']").GetAttributeValue("href", ""),
										OtherFormatsUrl = dtElement.SelectSingleNode("//a[@title='Other formats']").GetAttributeValue("href", ""),

										Title = GetInnerText(ddElement, "//div[@class='list-title mathjax']").Replace("Title:", "").Trim(),
										Comments = GetInnerText(ddElement, "//div[@class='list-comments mathjax']").Trim(),
										PrimarySubject = GetInnerText(ddElement, "//span[@class='primary-subject']").Trim(),
										Subjects = GetInnerText(ddElement, "//div[@class='list-subjects']").Trim(),
										Authors = ddElement.SelectNodes("//div[@class='list-authors']/a[@href]")
												  ?.ToDictionary(a => a.InnerText.Trim(), n => n.GetAttributeValue("href", "")),
									});

				foreach (var item in parsedQ)
				{
					ArticleScrapedDataDto dto = new ArticleScrapedDataDto();
					dto.DisplayDate = item.pub_date;
					dto.ArxivId = item.AbstractAnchorText;
					dto.AbstractUrl = item.AbstractUrl;
					dto.PdfUrl = item.pdfUrl;
					dto.OtherFormatUrl = item.OtherFormatsUrl;

					dto.Title = item.Title;
					dto.Comments = item.Comments;

					if (!string.IsNullOrEmpty(item.PrimarySubject))
					{
						dto.PrimarySubject = item.PrimarySubject.Replace(")", "").Split('(');
					}

					if (!string.IsNullOrEmpty(item.Subjects))
					{
						var subjectTextList = item.Subjects.Split(';');
						if (subjectTextList != null & subjectTextList.Length > 0)
						{
							foreach (var subjectStr in subjectTextList)
							{
								var arr = subjectStr.Replace(")", "").Split('(');
								dto.Subjects.Add(arr[1], arr[0]);
							}
						}
					};

					result.Add(dto);
				}
			}

			return result;
		}


		public static string GetInnerText(HtmlNode ddHtmlNode, string xpath)
		{
			try
			{
				if (ddHtmlNode != null)
				{
					return ddHtmlNode.SelectSingleNode(xpath).InnerText ?? "";
				}
			}
			catch (Exception) { }

			return string.Empty;
		}
	}
}
