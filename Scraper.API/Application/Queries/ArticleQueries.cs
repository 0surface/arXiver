using Scraper.API.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.API.Application.Queries
{
    public interface IArticleQueries
    {
        Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticlesAsync(string subjectCode);
    }

    public class ArticleQueries : IArticleQueries
    {
        private string _connectionString = string.Empty;
        private IArticleContext _context;

        public ArticleQueries(/*string constr*/)
        {
            _context = new ArticleContext();
            //_connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public ArticleQueries(IArticleContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticlesAsync(string subjectCode)
        {
            var subjectItem = _context.SubjectItems
                                    .FirstOrDefault(s => s.Code == subjectCode && s.IsPrimary);

            var articles = _context.SubjectItemArticles
                        .Where(j => j.SubjectItemId == subjectItem.Id && subjectItem.IsPrimary)
                        ?.Select(a => a.Article)
                        ?.ToList();

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

            return await Task.FromResult(vmList);
        }
    }
}
