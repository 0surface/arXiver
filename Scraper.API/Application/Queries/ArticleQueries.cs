using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.API.Application.Queries
{
    public interface IArticleQueries 
    {
        Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticleItemsAsync(string subjectId);
        
    }

    public class ArticleQueries : IArticleQueries
    {
        public Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticleItemsAsync(string subjectId)
        {
            throw new NotImplementedException();
        }
    }
}
