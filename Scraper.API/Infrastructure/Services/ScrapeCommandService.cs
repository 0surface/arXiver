using Microsoft.EntityFrameworkCore;
using Scraper.API.Infrastructure.Repositories;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Service.Scrapers;
using Scraper.Service.Util;
using Scraper.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.API.Infrastructure.Services
{
    public interface IScrapeCommandService
    {
        Task<ScrapeResultDto<Article>> SubmissionsBySubjectCodeAsync(string url, string subjectCode, CancellationToken cancellationToken);
        Task<ScrapeResultDto<Article>> SubmissionsBySubjectGroupAsync(string url, string subjectGroup, CancellationToken cancellationToken);
        Task<ScrapeResultDto<Article>> CatchupBySubjectGroupAsync(string url, CancellationToken cancellationToken);
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

        public async Task<ScrapeResultDto<Article>> SubmissionsBySubjectCodeAsync(string url, string subjectCode, CancellationToken cancellationToken)
        {
            ScrapeResultDto<Article> newSubmissionResult = new ScrapeResultDto<Article>();
            try
            {
                ScrapeResultDto<ArticleItemDto> scrapeResult =
                    await _articleListScraper.GetArticles(url, true, cancellationToken);

                newSubmissionResult.ContinueUrl = scrapeResult.ContinueUrl;
                newSubmissionResult.RequestUrl = scrapeResult.RequestUrl;

                List<Article> articles = MapArticlesToDomain(scrapeResult.Result);

                if (articles == null)
                {
                    return newSubmissionResult;
                }

                //Only include Submisisons
                List<Article> submissions = articles
                    .Where(a => a.ScrapeContext == ArticleScrapeContextEnum.Submission)
                    ?.ToList();

                //Fetch existing Articles scraped today
                var existingIds = _context.SubjectItemArticles
                                   .Where(j => j.Article.DisplayDate.Date == DateTime.Now.Date
                                           //&& j.SubjectItem.Code == subjectCode
                                           && j.SubjectItem.IsPrimary)
                                   ?.Select(a => a.Article.ArxivId)
                                   ?.ToList();

                //Filter out new articles not in database
                List<Article> newSubmissions = submissions
                            .Where(a => !existingIds.Contains(a.ArxivId))
                            .ToList();

                int repoResult = _repo.SaveBySubjectGroup(newSubmissions);
                newSubmissionResult.IsSucess = repoResult >= 1;
                newSubmissionResult.Result = newSubmissions;

                return newSubmissionResult;
            }
            catch (Exception ex)
            {
                newSubmissionResult.Exception = ex;
                return newSubmissionResult;
            }
        }

        public async Task<ScrapeResultDto<Article>> SubmissionsBySubjectGroupAsync(string url, string subjectGroup, CancellationToken cancellationToken)
        {
            ScrapeResultDto<Article> newSubmissionResult = new ScrapeResultDto<Article>();
            try
            {
                ScrapeResultDto<ArticleItemDto> scrapeResult =
                    await _articleListScraper.GetArticles(url, true, cancellationToken);

                newSubmissionResult.ContinueUrl = scrapeResult.ContinueUrl;
                newSubmissionResult.RequestUrl = scrapeResult.RequestUrl;

                List<Article> articles = MapArticlesToDomain(scrapeResult.Result);

                if (articles == null)
                {
                    return newSubmissionResult;
                }

                //Only include Submisisons
                List<Article> submissions = articles
                        .Where(a => a.ScrapeContext == ArticleScrapeContextEnum.Submission)
                        ?.ToList();

                //Fetch existing Articles scraped today
                var existingArxivIds = await _context.SubjectItemArticles
                                       .Where(j => j.Article.DisplayDate.Date == DateTime.Now.Date)
                                       ?.Select(a => a.Article.ArxivId)
                                       ?.ToListAsync();

                //Filter out new articles not in database
                List<Article> newSubmissions = submissions
                        .Where(a => !existingArxivIds.Contains(a.ArxivId))
                        .ToList();

                //Persist to Database
                int repoResult =  _repo.SaveBySubjectGroup(newSubmissions);
                newSubmissionResult.IsSucess = repoResult >= 1;
                newSubmissionResult.Result = newSubmissions;

                return newSubmissionResult;

            }
            catch (Exception ex)
            {
                newSubmissionResult.Exception = ex;
                return newSubmissionResult;
            }
        }

        public async Task<ScrapeResultDto<Article>> CatchupBySubjectGroupAsync(string url, CancellationToken cancellationToken)
        {
            ScrapeResultDto<Article> catcupResult = new ScrapeResultDto<Article>();

            try
            {
                ScrapeResultDto<ArticleItemDto> dtoResult =
                    await _articleListScraper.GetCatchUpArticles(url, true, cancellationToken);

                catcupResult.ContinueUrl = dtoResult.ContinueUrl;
                catcupResult.RequestUrl = dtoResult.RequestUrl;
              
                List<Article> articles = MapArticlesToDomain(dtoResult.Result);

                if (articles == null || articles.Count == 0)
                    return catcupResult;

                // Only include in Catchup only
                List<Article> catchups = catcupResult.Result
                        .Where(a => a.ScrapeContext == ArticleScrapeContextEnum.CatchUp)
                        ?.ToList();

                //Filter existing Articles
                var existingArxivIds = await _context.SubjectItemArticles
                                       .Where(j => j.Article.DisplayDate.Date == DateTime.Now.Date)
                                       ?.Select(a => a.Article.ArxivId)
                                       ?.ToListAsync();

                List<Article> newCatchups = catchups
                       .Where(a => !existingArxivIds.Contains(a.ArxivId))
                       .ToList();

                //Persist to Database
                int success =  _repo.SaveBySubjectGroup(newCatchups);

                catcupResult.IsSucess = success >= 1;
                catcupResult.Result = newCatchups;

                return catcupResult;
            }
            catch (Exception ex)
            {
                catcupResult.Exception =ex;
                return catcupResult;
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
