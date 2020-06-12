using Microsoft.EntityFrameworkCore;
using Scraper.API.Infrastructure.Repositories;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Service.Scrapers;
using Scraper.Service.Util;
using Scraper.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.API.Infrastructure.Services
{
    public interface IScrapeCommandService
    {
        Task<int> SubmissionsBySubjectCodeAsync(string url, string subjectCode, CancellationToken cancellationToken);
        Task<int> SubmissionsBySubjectGroupAsync(string url, string subjectGroup, CancellationToken cancellationToken);
    }

    public class ScrapeCommandService : IScrapeCommandService
    {
        private IArticleContext _context;
        private readonly IArticleListScraper _articleListScraper;
        private readonly IArticleItemRepository _repo;

        public ScrapeCommandService()
        {
            _context = new ArticleContext();
            _articleListScraper = new ArticleListScraper();
            _repo = new ArticleItemRepository();
        }

        public ScrapeCommandService(IArticleContext context
            , IArticleListScraper articleListScraper
            , IArticleItemRepository repo)
        {
            _context = context;
            _repo = repo;
            _articleListScraper = articleListScraper;
        }

        public async Task<int> SubmissionsBySubjectCodeAsync(string url, string subjectCode, CancellationToken cancellationToken)
        {
            try
            {
                List<ArticleItemDto> dtoList =
                    await _articleListScraper.GetArticles(url, true, true, cancellationToken);

                List<Article> articles = MapArticlesToDomain(dtoList);

                if (articles == null) return 0;

                //Filter Submisisons
                List<Article> submissions = articles
                    .Where(a => a.ScrapeContext == ArticleScrapeContextEnum.Submission)
                    ?.ToList();

                //Filter existing Articles
                var existingIds = _context.SubjectItemArticles
                                   .Where(j => j.Article.DisplayDate.Date == DateTime.Now.Date
                                           //&& j.SubjectItem.Code == subjectCode
                                           && j.SubjectItem.IsPrimary)
                                   ?.Select(a => a.Article.ArxivId)
                                   ?.ToList();

                var newSubmissions = submissions.Where(a => !existingIds.Contains(a.ArxivId)).ToList();

                return _repo.SaveBySubjectGroup(newSubmissions);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> SubmissionsBySubjectGroupAsync(string url, string subjectGroup, CancellationToken cancellationToken)
        {
            try
            {
                List<ArticleItemDto> dtoList =
                    await _articleListScraper.GetArticles(url, true, true, cancellationToken);

                List<Article> articles = MapArticlesToDomain(dtoList);

                if (articles == null) return 0;

                //Filter Submisisons
                List<Article> submissions = articles
                        .Where(a => a.ScrapeContext == ArticleScrapeContextEnum.Submission)
                        ?.ToList();

                //Filter existing Articles
                var existingArxivIds = await  _context.SubjectItemArticles
                                       .Where(j => j.Article.DisplayDate.Date == DateTime.Now.Date)
                                       ?.Select(a => a.Article.ArxivId)
                                       ?.ToListAsync();

                List<Article> newSubmissions = submissions
                        .Where(a => !existingArxivIds.Contains(a.ArxivId))
                        .ToList();

                //Persist to Database
                return _repo.SaveBySubjectGroup(newSubmissions);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #region Private
        
        private List<Article> MapArticlesToDomain(List<ArticleItemDto> dtoList)
        {
            List<Article> articles = new List<Article>();

            if (dtoList == null || dtoList.Count < 1)
                return articles;

            DateTime scrapedDate = DateTime.Now;

            var predefinedSubjectItems = _context.SubjectItems.ToList();

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
                        var existingPrimary = predefinedSubjectItems
                                    .FirstOrDefault(si =>
                                        si.Code == dto.PrimarySubject[1].Trim()
                                        && si.IsPrimary);

                        if (existingPrimary != null)
                        {
                            article.AddSubjectItem(existingPrimary);
                            dto.SubjectItems.RemoveAll(s => s.Code == existingPrimary.Code);
                        }
                        else
                        {
                            /*Only used if the database hasn't been seeded */
                            article.CreatePrimarySubject(dto.PrimarySubject[1].Trim()
                                                    , dto.PrimarySubject[0].Trim());
                            dto.SubjectItems.RemoveAll(s => s.Code == existingPrimary.Code);
                        }
                    }

                    if (dto.SubjectItems != null && dto.SubjectItems.Count > 0)
                    {
                        foreach (var sItem in dto.SubjectItems)
                        {
                            var existing = predefinedSubjectItems
                                                .FirstOrDefault(si =>
                                                si.Code == sItem.Code && si.IsPrimary == false);

                            if (existing != null)
                            {
                                article.AddSubjectItem(existing);
                            }
                            else
                            {
                                /*Only used if the database hasn't been seeded */
                                article.CreateSubjectItem(sItem.Code.Trim(),
                                                        sItem.Description.Trim());
                            }
                        }
                    }

                    article.AddDisplayDate(GetDisplayDateString(dto.H3HeaderText, dto.DateContextInfo));

                    article.AddScrapeContext(dto.PageHeaderInfo, dto.ArxivIdLabel, dto.H3HeaderText);

                    articles.Add(article);
                }
                catch (Exception) { }
            }

            return articles;
        }

        private string GetDisplayDateString(string h3Text, string dateInfo)
        {
            if (string.IsNullOrEmpty(h3Text) && string.IsNullOrEmpty(dateInfo))
                return string.Empty;

            if (!string.IsNullOrEmpty(dateInfo))
                return dateInfo;

            string[] sep = { "for" };

            string[] arr = h3Text.Split(sep, StringSplitOptions.None);

            return arr.Length > 1 ? arr[1].Trim() : string.Empty;
        }

        #endregion Private
    }
}
