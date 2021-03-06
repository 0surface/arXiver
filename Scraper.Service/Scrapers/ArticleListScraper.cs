﻿using HtmlAgilityPack;
using Scraper.Service.Util;
using Scraper.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Service.Scrapers
{
    public interface IArticleListScraper
    {
        int GetArticleListEntryCount(HtmlDocument htmlDocument, string[] identifierStrings
            , string articleListCountSelector = "//small");

        ScrapeResultDto<ArticleItemDto> ScrapeArticleList(ScrapeResultDto<ArticleItemDto> request, HtmlDocument htmlDocument, bool includeAbstract = true);

        ScrapeResultDto<ArticleItemDto> ScrapeCatchUpArticleList(ScrapeResultDto<ArticleItemDto> request, HtmlDocument htmlDocument, bool includeAbstract);

        Task<ScrapeResultDto<ArticleItemDto>> GetArticles(string url, bool includeAbstract, CancellationToken cancellationToken);

        Task<ScrapeResultDto<ArticleItemDto>>  GetCatchUpArticles(string url, bool includeAbstract, CancellationToken cancellationToken);
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

        public ScrapeResultDto<ArticleItemDto> ScrapeCatchUpArticleList(ScrapeResultDto<ArticleItemDto> request, HtmlDocument htmlDocument, bool includeAbstract = true)
        {
            try
            {
                var parsedHtml = (from info in htmlDocument.DocumentNode.SelectNodes("//div[@id='dlpage']")
                                  from dlElement in info.SelectNodes("dl")
                                  let olElement = info.SelectSingleNode("ol")
                                  let h1Page = info.SelectSingleNode("h1")
                                  let parentH3 = dlElement.GetPreviousSibling("h2")
                                  select new
                                  {
                                      ol = olElement,
                                      h1 = h1Page,
                                      date_info = parentH3,
                                      dl = dlElement
                                  }).ToList();

                if (parsedHtml == null || parsedHtml.Count < 0)
                    return request;

                request.ContinueUrl = GetCatchUpContinueUrl(parsedHtml[0]?.ol?.ChildNodes);

                //Html Page title information -used to eastablish the Scrape Context Enum (New, Recent, Catchup etc...)
                var pageInfo = parsedHtml[0].h1?.InnerText;

                for (int i = 0; i < parsedHtml.Count(); i++)
                {
                    var list = ProcessCatchUpElements(
                       pageInfo,
                       parsedHtml[i].date_info?.InnerText,
                       parsedHtml[i].dl.Descendants("dt").ToArray(),
                       parsedHtml[i].dl.Descendants("dd").ToArray(),
                       true);
                    request.Result.AddRange(list);
                }
            }
            catch (Exception) { }

            return request;
        }

        public async Task<ScrapeResultDto<ArticleItemDto>> GetCatchUpArticles(string url, bool includeAbstract, CancellationToken cancellationToken)
        {
            ScrapeResultDto<ArticleItemDto> result = new ScrapeResultDto<ArticleItemDto>() { RequestUrl = url };

            HtmlDocument doc = await HtmlAgilityHelper.GetHtmlDocument(url, cancellationToken);

            return (doc == null) ?
                result
                : ScrapeCatchUpArticleList(result, doc, includeAbstract);
        }

        public ScrapeResultDto<ArticleItemDto> ScrapeArticleList(ScrapeResultDto<ArticleItemDto> request, HtmlDocument htmlDocument, bool includeAbstract = true)
        {
            try
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

                if (parseMain == null || parseMain.Count() < 1)
                    return request;

                for (int i = 0; i < parseMain.Count(); i++)
                {
                    var h3Elem = parseMain[i].h3.InnerText;
                    var dt_list = parseMain[i].dl.Descendants("dt").ToArray();
                    var dd_list = parseMain[i].dl.Descendants("dd").ToArray();

                    var list = ProcessElems(h3Elem, dt_list, dd_list, includeAbstract);

                    if (list != null && list.Count > 0)
                    {
                        request.Result.AddRange(list);
                        request.IsSucess = true;
                    }
                }
            }
            catch (Exception)
            {
                request.IsSucess = false;
            }

            return request;
        }

        public async Task<ScrapeResultDto<ArticleItemDto>> GetArticles(string url, bool includeAbstract, CancellationToken cancellationToken)
        {
            ScrapeResultDto<ArticleItemDto> result = new ScrapeResultDto<ArticleItemDto>() { RequestUrl = url };

            HtmlDocument htmlDocument = await HtmlAgilityHelper.GetHtmlDocument(url, cancellationToken);

            return htmlDocument == null ? result :  ScrapeArticleList(result, htmlDocument, includeAbstract);
        }
        
        #region Private

        private List<ArticleItemDto> ProcessCatchUpElements(string pageTitleInfo, string dateInfo,
                                    HtmlNode[] dtelem_list, HtmlNode[] ddelem_list, bool includeAbstract)
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
                    arxivIdInnerText = dtelem_list[i]?.InnerText,
                    dtdesc = dtelem_list[i].Descendants("a"),
                    dddesc = ddelem_list[i].Descendants()
                };

                ArticleItemDto dto = new ArticleItemDto();

                dto.PageHeaderInfo = pageTitleInfo;
                dto.DateContextInfo = dateInfo;

                dto.ArxivIdLabel = GetDtArxivIdLabel(item.arxivIdInnerText);
                dto.ArxivId = GetDtElementInnerText(item.dtdesc, "Abstract");
                dto.AbstractUrl = GetDtElementAttributeValue(item.dtdesc, "Abstract");
                dto.PdfUrl = GetDtElementAttributeValue(item.dtdesc, "Download PDF");
                dto.OtherFormatUrl = GetDtElementAttributeValue(item.dtdesc, "Other formats");

                dto.Title = GetDDElementTitle(item.dddesc, "div", "class", "list-title mathjax", "Title:");
                dto.Comments = GetDDElementComment(item.dddesc, "list-comments mathjax");
                dto.AbstractText = includeAbstract ? GetDDElementAbstract(item.dddesc, "mathjax") : string.Empty;
                dto.PrimarySubject = GetDDElementPrimarySubject(item.dddesc, "div", "class", "list-subjects", ")");

                dto.SubjectItems = GetDDElementSubjects(item.dddesc, "div", "class", "list-subjects");
                dto.Authors = GetDDElementAuthors(item.dddesc, "div", "class", "list-authors");

                result.Add(dto);
            }

            return result;
        }

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
                dto.ArxivId = GetDtElementInnerText(item.dtdesc, "Abstract");
                dto.AbstractUrl = GetDtElementAttributeValue(item.dtdesc, "Abstract");
                dto.PdfUrl = GetDtElementAttributeValue(item.dtdesc, "Download PDF");
                dto.OtherFormatUrl = GetDtElementAttributeValue(item.dtdesc, "Other formats");

                dto.Title = GetDDElementTitle(item.dddesc, "div", "class", "list-title mathjax", "Title:");
                dto.Comments = GetDDElementComment(item.dddesc, "list-comments mathjax");
                dto.AbstractText = includeAbstract ? GetDDElementAbstract(item.dddesc, "mathjax") : string.Empty;
                dto.PrimarySubject = GetDDElementPrimarySubject(item.dddesc, "div", "class", "list-subjects", ")");

                dto.SubjectItems = GetDDElementSubjects(item.dddesc, "div", "class", "list-subjects");
                dto.Authors = GetDDElementAuthors(item.dddesc, "div", "class", "list-authors");

                result.Add(dto);
            }

            return result;
        }

        private List<AuthorDto> GetDDElementAuthors(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue)
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

        private List<SubjectItemDto> GetDDElementSubjects(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue, string repalceWith = "")
        {
            var subjNode = nodes
                ?.Where(a => a.HasAttributes && a.Name == tag && a.Attributes.AttributeExists("class", attributeValue))
                ?.FirstOrDefault();

            return subjNode != null ?
                GetSubjectArray(subjNode.InnerText?.Trim())
                : new List<SubjectItemDto>();
        }

        private string[] GetDDElementPrimarySubject(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue, string repalceWith = "")
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

        private string GetDDElementTitle(IEnumerable<HtmlNode> nodes, string tag, string attribute, string attributeValue, string repalceWith = "")
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

        private string GetDDElementComment(IEnumerable<HtmlNode> nodes, string className)
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

        private string GetDDElementAbstract(IEnumerable<HtmlNode> nodes, string className)
        {
            if (nodes != null && nodes.Count() > 0)
            {
                var n = nodes.Where(a => a.Name == "p" && a.Attributes.AttributeExists("class", className))
                    ?.FirstOrDefault();

                return n?.InnerText?.Trim() ?? "";

            }
            return "";
        }

        private string GetDtElementAttributeValue(IEnumerable<HtmlNode> nodes, string titleValue)
        {
            if (nodes != null && nodes.Count() > 0)
            {
                return nodes
                    ?.Where(a => a.Name == "a" && a.Attributes.AttributeExists("title", titleValue))?.FirstOrDefault()
                    ?.GetAttributeValue("href", "") ?? "";
            }
            return "";
        }

        private string GetDtElementInnerText(IEnumerable<HtmlNode> nodes, string titleValue)
        {
            if (nodes != null && nodes.Count() > 0)
            {
                return nodes
                    ?.Where(a => a.Name == "a" && a.Attributes.AttributeExists("title", titleValue))?.FirstOrDefault()
                    ?.InnerText ?? string.Empty;
            }
            return string.Empty;
        }

        private string GetDtArxivIdLabel(string arxivIdInnerText)
        {
            return (arxivIdInnerText.Contains('(') | arxivIdInnerText.Contains(')')) ?
                arxivIdInnerText.RegexFindInBetweenStrings("(", ")")
                : string.Empty;
        }

        private string GetCatchUpContinueUrl(IEnumerable<HtmlNode> nodes)
        {
            return WebUtility.HtmlDecode(nodes
                 ?.Where(n => n.Name == "li"
                            && n.InnerText.ToLower().Contains("continue"))
                 ?.FirstOrDefault()
                 ?.ChildNodes
                 ?.Where(cn => cn.Name == "a")
                 ?.FirstOrDefault()
                 ?.GetAttributeValue("href", "")) ?? string.Empty;
        }

        private List<SubjectItemDto> GetSubjectArray(string input)
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

        #endregion Private
    }
}
