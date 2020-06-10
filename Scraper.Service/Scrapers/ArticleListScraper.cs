﻿using HtmlAgilityPack;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Service.Util;
using Scraper.Types.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Service.Scrapers
{
	public interface IArticleListScraper
	{
		int GetArticleListEntryCount(HtmlDocument htmlDocument, string[] identifierStrings
			, string articleListCountSelector = "//small");

		List<ArticleItemDto> GetArticleList(HtmlDocument htmlDocument, bool includeAbstract);

		Task<List<Article>> GetArticles(string url, bool includeAbstract, CancellationToken cancellationToken);
	}

	public class ArticleListScraper : IArticleListScraper
	{
		public ArticleListScraper()
		{
			//TODO : Inject xpath repo
		}

		public int GetArticleListEntryCount(HtmlDocument htmlDocument, string[] identifierStrings
			, string articleListCountSelector = "//small")
		{
			int result = 0;

			try
			{
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

					int.TryParse(splitText.SafeRegexReplace("[^0-9.]", ""), out result);
				}

				return result;
			}
			catch (Exception)
			{
				return -1;
			}
		}

		public List<ArticleItemDto> GetArticleList(HtmlDocument htmlDocument, bool includeAbstract = true)
		{
			var parseMain = (from info in htmlDocument.DocumentNode.SelectNodes("//div[@id='dlpage']")
							 from h3Element in info.SelectNodes("h3")
							 let dlElement = h3Element.GetNextSibling("dl")
							 let dte = dlElement.SelectNodes($"//span[@class='list-identifier']")
							 let dde = dlElement.SelectNodes($"//div[@class='meta']")
							 where dlElement != null
							 select new
							 {
								 h3 = h3Element,
								 dl = dlElement,

								 dt = dlElement.Descendants("dt"),
								 dd = dlElement.Descendants("dd")
							 }).ToList();

			List<ArticleItemDto> result = new List<ArticleItemDto>();

			int loopCount = parseMain.Count();

			for (int i = 0; i < loopCount; i++)
			{
				var h3Elem = parseMain[i].h3.InnerText;
				var dt_list = parseMain[i].dl.Descendants("dt").ToArray();
				var dd_list = parseMain[i].dl.Descendants("dd").ToArray();

				var list = ProcessElems(h3Elem, dt_list, dd_list, includeAbstract);

				if (list != null && list.Count > 0)
				{
					result.AddRange(list);
				}
			}

			return result;
		}

		public async Task<List<Article>> GetArticles(string url, bool includeAbstract, CancellationToken cancellationToken)
		{			
			HtmlDocument doc = await HtmlAgilityHelper.GetHtmlDocument(url, cancellationToken);

			return (doc != null) ? 
				MaptoDomain( GetArticleList(doc, includeAbstract))
				: new List<Article>();
		}


		private List<Article> MaptoDomain(List<ArticleItemDto> dtoList)
		{
			List<Article> articles = new List<Article>();

			if (dtoList == null || dtoList.Count < 1)return articles;

			DateTime scrapedDate = DateTime.Now;

			foreach (var item in dtoList)
			{
				try
				{
					Article article = new Article(item.ArxivId,
						item.AbstractUrl,
						item.PdfUrl,
						item.OtherFormatUrl,
						item.Title,
						item.AbstractText,
						item.Comments,
						string.Empty,
						string.Empty,
						scrapedDate);

					if (item.Authors != null && item.Authors.Count > 0)
					{
						foreach (var author in item.Authors)
						{	
							article.AddAuthor(author.FullName
								, author.ContextUrl?.RegexFindInBetweenStrings("+", "/"));
						}
					}

					if (item.PrimarySubject != null && item.PrimarySubject.Length == 2)
					{
						article.AddPrimarySubject(item.PrimarySubject[1], item.PrimarySubject[0]);
					}

					if (item.Subjects != null && item.Subjects.Count > 0)
					{
						foreach (var subject in item.Subjects)
						{
							article.AddSubjectItem(subject.Code, subject.Description);
						}
					}

					article.AddDisplayDate(GetDisplayDateString(item.H3HeaderText));

					article.AddScrapeContext(item.H3HeaderText);

					articles.Add(article);
				}
				catch (Exception ex)
				{
					var c = ex.Message;
				}
			}

			return articles;
		}

		#region Private

		private List<ArticleItemDto> ProcessElems(string h3text, HtmlNode[] dtelem_list, HtmlNode[] ddelem_list, bool includeAbstract)
		{
			List<ArticleItemDto> result = new List<ArticleItemDto>();

			if (dtelem_list == null || dtelem_list.Length < 1
				|| ddelem_list == null || ddelem_list.Length < 1)
				return result;

			int loopCount = Math.Min(dtelem_list.Count(), ddelem_list.Count());

			for (int i = 0; i < loopCount; i++)
			{
				var item = new
				{
					pub_date = h3text,
					dtdesc = dtelem_list[i].Descendants("a"),
					dddesc = ddelem_list[i].Descendants()
				};

				ArticleItemDto dto = new ArticleItemDto();

				dto.H3HeaderText = item.pub_date;
				dto.ArxivId = _GetDtElementInnerText(item.dtdesc, "Abstract");
				dto.AbstractUrl = _GetDtElementAttributeValue(item.dtdesc, "Abstract");
				dto.PdfUrl = _GetDtElementAttributeValue(item.dtdesc, "Download PDF");
				dto.OtherFormatUrl = _GetDtElementAttributeValue(item.dtdesc, "Other formats");

				dto.Title = _GetDDElementTitle(item.dddesc, "div", "class", "list-title mathjax", "Title:");
				dto.Comments = _GetDDElementComment(item.dddesc, "list-comments mathjax");
				dto.AbstractText = includeAbstract ? _GetDDElementAbstract(item.dddesc, "mathjax") : string.Empty;
				dto.PrimarySubject = _GetDDElementPrimarySubject(item.dddesc, "div", "class", "list-subjects", ")");

				dto.Subjects = _GetDDElementSubjects(item.dddesc, "div", "class", "list-subjects");
				dto.Authors = _GetDDElementAuthors(item.dddesc, "div", "class", "list-authors");

				result.Add(dto);
			}

			return result;
		}

		private List<AuthorDto> _GetDDElementAuthors(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue)
		{
			var authorNodes = nodes
				?.Where(a => a.HasAttributes && a.Name == tag && a.Attributes.AttributeExists("class", attributeValue))
				?.FirstOrDefault()
				?.Descendants()
				?.Where(a => a.Name == "a")
				?.ToList();

			List<AuthorDto> authors = new List<AuthorDto>();
			if (authorNodes == null || authorNodes.Count < 1)
				return authors;

			foreach (var node in authorNodes)
			{
				authors.Add(new AuthorDto()
				{
					FullName = node.InnerText.Trim(),
					ContextUrl = node.GetAttributeValue("href", "")
				});
			}

			return authors;

		}

		private List<SubjectItemDto> _GetDDElementSubjects(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue, string repalceWith = "")
		{
			var subjNode = nodes
				?.Where(a => a.HasAttributes && a.Name == tag && a.Attributes.AttributeExists("class", attributeValue))
				?.FirstOrDefault();

			return subjNode != null ?
				_GetSubjectArray(subjNode.InnerText?.Trim())
				: new List<SubjectItemDto>();
		}

		private string[] _GetDDElementPrimarySubject(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue, string repalceWith = "")
		{
			var subjNode = nodes
				?.Where(a => a.HasAttributes && a.Name == tag && a.Attributes.AttributeExists("class", attributeValue))
				?.FirstOrDefault();

			return subjNode?.Descendants()
				?.Where(a => a.Name == "span" && a.Attributes.AttributeExists("class", "primary-subject"))?.FirstOrDefault()
				?.InnerText
				.Replace(repalceWith, "")
				?.Split('(') ?? new string[2];
		}

		private string _GetDDElementTitle(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue, string repalceWith = "")
		{
			if (nodes != null && nodes.Count() > 0)
			{
				return nodes.Where(a => a.HasAttributes && a.Name == tag && a.Attributes.AttributeExists("class", attributeValue))
					?.FirstOrDefault()
					?.InnerText
					?.Replace(repalceWith, "")
					?.Trim() ?? "";
			}
			return "";
		}

		private string _GetDDElementComment(IEnumerable<HtmlNode> nodes, string className)
		{
			if (nodes != null && nodes.Count() > 0)
			{
				return nodes.Where(a => a.Name == "div" && a.HasAttributes && a.Attributes.AttributeExists("class", className))
					?.FirstOrDefault()
					?.InnerText
					?.Replace("Comments:", "")
					?.Trim() ?? "";

			}
			return "";
		}

		private string _GetDDElementAbstract(IEnumerable<HtmlNode> nodes, string className)
		{
			if (nodes != null && nodes.Count() > 0)
			{
				var n = nodes.Where(a => a.Name == "p" && a.Attributes.AttributeExists("class", className))
					?.FirstOrDefault();

				return n?.InnerText?.Trim() ?? "";

			}
			return "";
		}

		private string _GetDtElementAttributeValue(IEnumerable<HtmlNode> nodes, string titleValue)
		{
			if (nodes != null && nodes.Count() > 0)
			{
				return nodes
					?.Where(a => a.Name == "a" && a.Attributes.AttributeExists("title", titleValue))?.FirstOrDefault()
					//?.Where(a => a.HasAttributes && a.Attributes["title"].Value == titleValue)
					//?.FirstOrDefault()
					?.GetAttributeValue("href", "") ?? "";
			}
			return "";
		}

		private string _GetDtElementInnerText(IEnumerable<HtmlNode> nodes, string titleValue)
		{
			if (nodes != null && nodes.Count() > 0)
			{
				return nodes
					?.Where(a => a.Name == "a" && a.Attributes.AttributeExists("title", titleValue))?.FirstOrDefault()
					//?.Where(a => a.HasAttributes && a.Attributes["title"].Value == titleValue)
					//?.Where(a => a.HasAttributes && a.Attributes["title"].Value == titleValue)
					//?.FirstOrDefault()
					?.InnerText ?? "";
			}
			return "";
		}

		public void ViewNodes(HtmlDocument htmlDocument)
		{
			//var dt_Title_nodes = htmlDocument.DocumentNode.SelectNodes($"//div[@id='dlpage']/dl/dt//div[@class='list-identifier']//a[@title='Abstract']");//?.FirstOrDefault().InnerText ?? "";

			var firstdl = htmlDocument.DocumentNode.SelectSingleNode($"//div[@id='dlpage']/dl");
			var firsth3 = firstdl.PreviousSibling;
			var seconddl = firstdl.NextSibling;

			var pageNodes = htmlDocument.DocumentNode.SelectNodes($"//div[@id='dlpage']");
			var dl_Title_nodes = htmlDocument.DocumentNode.SelectNodes($"//div[@id='dlpage']/dl");
			var dt_nodes = htmlDocument.DocumentNode.SelectNodes($"//div[@id='dlpage']/dl/dt");
			var dd_nodes = htmlDocument.DocumentNode.SelectNodes($"//div[@id='dlpage']/dl/dd");
			var h3_nodes = htmlDocument.DocumentNode.SelectNodes($"//div[@id='dlpage']/h3");

			var pc = pageNodes.Count;
			var count_dl = dl_Title_nodes.Count;
			var count_dt = dt_nodes.Count;



			foreach (var item in pageNodes)
			{
				var parent = item.ParentNode;
				var grandParent = parent.ParentNode;
				var prev = item.PreviousSibling;
				var next = item.NextSibling;
				var child = item.ChildNodes;
			}

			foreach (var item in dl_Title_nodes)
			{
				var parent = item.ParentNode;
				var grandParent = parent.ParentNode;
				var prev = item.PreviousSibling;
				var next = item.NextSibling;
				var child = item.ChildNodes;
			}

			foreach (var item in dt_nodes)
			{
				var parent = item.ParentNode;
				var grandParent = parent.ParentNode;

				var ddelem = item.GetNextSibling("dd");

				var prev = item.PreviousSibling;
				var next = item.NextSibling;
				var child = item.ChildNodes;
			}

			foreach (var item in dd_nodes)
			{
				var parent = item.ParentNode;
				var grandParent = parent.ParentNode;
				var prev = item.PreviousSibling;
				var next = item.NextSibling;
				var child = item.ChildNodes;
			}

		}

		private List<Tuple<string, HtmlNode>> GetArticlesByPublicationDate(HtmlDocument htmlDocument)
		{
			List<Tuple<string, HtmlNode>> result = new List<Tuple<string, HtmlNode>>();
			try
			{
				var parseMain = (from info in htmlDocument.DocumentNode.SelectNodes("//div[@id='dlpage']")
								 from h3Element in info.SelectNodes("h3")
								 let dlElement = h3Element.GetNextSibling("dl")
								 where dlElement != null
								 select new
								 {
									 h3 = h3Element.InnerText,
									 dl = dlElement
								 }).ToList();

				foreach (var item in parseMain)
				{
					Tuple<string, HtmlNode> publicationDate =
						new Tuple<string, HtmlNode>(item.h3, item.dl);
					result.Add(publicationDate);
				}

			}
			catch (Exception ex)
			{
				var c = ex.Message;
			}

			return result;
		}

		private List<SubjectItemDto> _GetSubjectArray(string input)
		{
			List<SubjectItemDto> dto
			   = new List<SubjectItemDto>();

			if (string.IsNullOrEmpty(input)) return dto;

			input.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				 .Select(x => x.Split(new char[] { '(' }).ToArray())
					.ToList()
					.ForEach(arr => dto.Add(new SubjectItemDto()
					{
						Description = arr[0].Replace("Subjects:", ""),
						Code = arr.Length > 1 ? arr[1].Replace(")", "") : string.Empty,
					}));

			return dto;
		}

		private List<AuthorDto> _GetAuthors(HtmlNode authorNode)
		{
			List<AuthorDto> authors = new List<AuthorDto>();
			var authorNodes = authorNode.ChildNodes.Where(n => n.Name == "a").ToList();

			if (authorNodes == null || authorNodes.Count < 1)
				return authors;

			foreach (var node in authorNodes)
			{
				authors.Add(new AuthorDto()
				{
					FullName = node.InnerText.Trim(),
					ContextUrl = node.GetAttributeValue("href", "")
				});
			}

			return authors;
		}

		private string GetDisplayDateString(string h3Text)
        {
			if (string.IsNullOrEmpty(h3Text))
				return string.Empty;

			string[] arr = h3Text.Split("for".ToCharArray(), StringSplitOptions.None);

			return arr.Length > 1 ? arr[1].Trim() : string.Empty;
		}

		#endregion Private
	}
}
