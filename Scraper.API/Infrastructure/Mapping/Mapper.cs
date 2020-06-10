using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Service.Util;
using Scraper.Types.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.API.Infrastructure.Mapping
{
    public interface IScrapeMapper
    {
        List<Article> MapArticleToDomain(List<ArticleItemDto> dtoList);
    }

    public class ScrapeMapper : IScrapeMapper
    {
        private IArticleContext _context;

        public ScrapeMapper()
        {
            _context = new ArticleContext();
        }

        public ScrapeMapper(IArticleContext context)
        {
            _context = context;
        }

        public List<Article> MapArticleToDomain(List<ArticleItemDto> dtoList)
        {
            List<Article> articles = new List<Article>();

            if (dtoList == null || dtoList.Count < 1)
                return articles;

            DateTime scrapedDate = DateTime.Now;

            var existingSubjectItems = _context.SubjectItems;

            foreach (var dto in dtoList)
            {
                try
                {
                    Article article = new Article(dto.ArxivId,
                        dto.AbstractUrl,
                        dto.PdfUrl,
                        dto.OtherFormatUrl,
                        dto.Title,
                        dto.AbstractText,
                        dto.Comments,
                        string.Empty,
                        string.Empty,
                        scrapedDate);

                    if (dto.Authors != null && dto.Authors.Count > 0)
                    {
                        foreach (var author in dto.Authors)
                        {
                            article.AddAuthor(author.FullName.Trim()
                                , author.ContextUrl?.RegexFindInBetweenStrings("+", "/"));
                        }
                    }

                    if (dto.PrimarySubject != null && dto.PrimarySubject.Length == 2)
                    {
                        var existingPrimary = existingSubjectItems
                            .FirstOrDefault(si =>
                                    si.Code == dto.PrimarySubject[1].Trim()
                                    && si.IsPrimary);

                        if (existingPrimary != null)
                        {
                            article.AddExistingSubjectItem(existingPrimary);
                        }
                        else
                        {
                            article.AddPrimarySubject(dto.PrimarySubject[1].Trim()
                                                    , dto.PrimarySubject[0].Trim());
                        }
                    }

                    if (dto.SubjectItems != null && dto.SubjectItems.Count > 0)
                    {
                        foreach (var sItem in dto.SubjectItems)
                        {
                            var existing = existingSubjectItems
                                                .FirstOrDefault(si =>
                                                si.Code == sItem.Code && si.IsPrimary == false);

                            if (existing != null)
                            {
                                article.AddExistingSubjectItem(existing);
                            }
                            else
                            {
                                article.AddSubjectItem(sItem.Code.Trim(),
                                                        sItem.Description.Trim());
                            }
                        }
                    }

                    article.AddDisplayDate(GetDisplayDateString(dto.H3HeaderText));

                    article.AddScrapeContext(dto.H3HeaderText);

                    articles.Add(article);
                }
                catch (Exception) { }
            }

            return articles;
        }

        private string GetDisplayDateString(string h3Text)
        {
            if (string.IsNullOrEmpty(h3Text))
                return string.Empty;

            string[] sep = { "for" };

            string[] arr = h3Text.Split(sep, StringSplitOptions.None);

            return arr.Length > 1 ? arr[1].Trim() : string.Empty;
        }
    }
}
