using HtmlAgilityPack;
using Scraper.Service.Util;
using Scraper.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Service.Scrapers
{
    public interface IArticleListScraper
    {
        int GetArticleListEntryCount(HtmlDocument htmlDocument, string[] identifierStrings
            , string articleListCountSelector = "//small");

        List<ArticleItemDto> GetArticleList(HtmlDocument htmlDocument, bool includeAbstract);

        List<ArticleItemDto> GetCatchUpArticleList(HtmlDocument htmlDocument);

        Task<List<ArticleItemDto>> GetArticles(string url, bool includeAbstract, bool submissionOnly, CancellationToken cancellationToken);
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

        public List<ArticleItemDto> GetCatchUpArticleList(HtmlDocument htmlDocument)
        {
            List<ArticleItemDto> result = new List<ArticleItemDto>();

            try
            {
                var parsedHtml = (from info in htmlDocument.DocumentNode.SelectNodes("//div[@id='dlpage']")
                                  from dlElement in info.SelectNodes("dl")
                                  let h1Page = info.SelectSingleNode("h1")
                                  let h2ElemDate = info.SelectSingleNode("h2")
                                  let h3ElemContext = info.SelectSingleNode("h3")
                                  select new
                                  {
                                      h1 = h1Page,
                                      h2 = h2ElemDate,
                                      h3 = h3ElemContext,
                                      dl = dlElement
                                  }).ToList();

                if (parsedHtml == null || parsedHtml.Count < 0)
                    return result;

                string pageInfo = parsedHtml[0].h1?.InnerText ?? string.Empty;
                string dateInfo = parsedHtml[0].h2?.InnerText ?? string.Empty;

                for (int i = 0; i < parsedHtml.Count; i++)
                {
                    var list = ProcessCatchUpElements(
                        pageInfo,
                        dateInfo,
                        parsedHtml[i].dl.Descendants("dt").ToArray(),
                        parsedHtml[i].dl.Descendants("dd").ToArray(),
                        true);

                    result?.AddRange(list);
                }
            }
            catch (Exception) { }

            return result;
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

        public async Task<List<ArticleItemDto>> GetArticles(string url, bool includeAbstract, bool submissionOnly, CancellationToken cancellationToken)
        {
            HtmlDocument doc = await HtmlAgilityHelper.GetHtmlDocument(url, cancellationToken);

            if (doc == null)
                return new List<ArticleItemDto>();

            var articles = GetArticleList(doc, includeAbstract);

            return articles != null && articles.Count > 0
                ? articles
                : new List<ArticleItemDto>();
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
                    //?.Where(a => a.HasAttributes && a.Attributes["title"].Value == titleValue)
                    //?.FirstOrDefault()
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
                :string.Empty;
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

        private List<AuthorDto> GetAuthors(HtmlNode authorNode)
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

        #endregion Private
    }
}
