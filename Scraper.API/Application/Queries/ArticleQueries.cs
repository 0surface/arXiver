using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scraper.API.Extensions;
using Scraper.API.Infrastructure;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace Scraper.API.Application.Queries
{
    public interface IArticleQueries
    {
        Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticlesAsync(string subjectCode);
        Task<IEnumerable<ArticleItemVM>> GetArticlesAsync(string subjectCode);
    }

    public class ArticleQueries : IArticleQueries
    {
        private IArticleContext _context;
        private readonly ILogger<ArticleQueries> _logger;

        public ArticleQueries()
        {
            _context = new ArticleContext();
        }

        public ArticleQueries(IArticleContext context, ILogger<ArticleQueries> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticlesAsync(string subjectCode)
        {
            var subjectItem = _context.SubjectItems
                                    .FirstOrDefault(s => s.Code == subjectCode && s.IsPrimary);

            if (subjectItem == null) 
                return await Task.FromResult(new List<ArticleItemSummaryVM>());

            var articles = _context.SubjectItemArticles
                        .Where(j => j.SubjectItemId == subjectItem.Id && subjectItem.IsPrimary)
                        ?.Select(a => a.Article)
                        ?.ToList();

            List<ArticleItemSummaryVM> vmList =   MapToSummaryArticleViewModels(subjectItem, articles);

            return await Task.FromResult(vmList);
        }

        public async Task<IEnumerable<ArticleItemVM>> GetArticlesAsync(string subjectCode)
        {
            var primarySubject = _context.SubjectItems
                                    .FirstOrDefault(s => s.Code == subjectCode && s.IsPrimary);

            var articles = (from article in _context.Articles
                            join subItemArticle in _context.SubjectItemArticles on article.Id equals subItemArticle.ArticleId
                            where subItemArticle.SubjectItemId == primarySubject.Id 
                            select article )
                            .Include(a => a.AuthorArticles).ThenInclude( a => a.Author)
                            .Include(a => a.SubjectItemArticles).ThenInclude(a => a.SubjectItem)
                            .ToList();                       

            List<ArticleItemVM> vmList = MapToArticleViewModels(primarySubject, articles);

            return await Task.FromResult(vmList);
        }

        #region Private

        private static List<ArticleItemSummaryVM> MapToSummaryArticleViewModels(SubjectItem subjectItem, List<Article> articles)
        {
            List<ArticleItemSummaryVM> vmList = new List<ArticleItemSummaryVM>();

            if (articles != null)
            {
                foreach (var article in articles)
                {
                    vmList.Add(new ArticleItemSummaryVM()
                    {
                        ArxivId = article.ArxivId,
                        Title = article.Title,
                        SubjectCode = subjectItem.Code,
                        SubjectName = subjectItem.Name
                    });
                }
            }

            return vmList;
        }

        private List<ArticleItemVM> MapToArticleViewModels(SubjectItem primarySubject, List<Article> articles)
        {
            List<ArticleItemVM> vmList = new List<ArticleItemVM>();

            if (articles != null)
            {
                foreach (var article in articles)
                {
                    ArticleItemVM newItem = new ArticleItemVM()
                    {
                        ArxivId = article.ArxivId,
                        Title = article.Title,
                        AbstractUrl = article.HtmlLink,
                        PdfUrl = article.PdfUrl,
                        OtherFormatUrl = article.OtherFormatUrl,
                        Comments = article.Comments,
                        AbstractText = article.AbstractText,
                        DisplayDate = article.DisplayDate,
                        ScrapedDate =article.ScrapedDate,

                        PrimarySubject = new SubjectItemVM()
                        {
                            Code = primarySubject.Code,
                            Name = primarySubject.Name
                        },
                        Subjects = article.SubjectItemArticles.Select(s => s.SubjectItem)
                                        .Where( s => s.Code != primarySubject.Code)
                                        .SelectTry(s => CreateSubjectItemVM(s))
                                        .OnCaughtException(ex => { _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null)
                                        .ToList()
                        ,
                        Authors = article.AuthorArticles.Select(a => a.Author)
                                        .SelectTry(s => CreateAuthorVM(s))
                                        .OnCaughtException(ex => { _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x != null)
                                        .ToList()
                    };

                    vmList.Add(newItem);
                }
                
            }

            return vmList;
        }

        private static AuthorVM CreateAuthorVM (Author domain)
        {
            return domain == null 
                ? null : new AuthorVM()
            {
                Code = domain.Code,
                Name = domain.Name
            };
        }

        private static SubjectItemVM CreateSubjectItemVM(SubjectItem domain)
        {
            return domain == null 
                ? null : new SubjectItemVM()
            {
                Code = domain.Code,
                Name = domain.Name
            };
        }

        #endregion Private
    }
}
