using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scraper.API.Application.Queries
{
    public interface IArticleQueries
    {
        Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticleItemsAsync(string subjectId);
    }

    public class ArticleQueries : IArticleQueries
    {
        private string _connectionString = string.Empty;

        public ArticleQueries(/*string constr*/)
        {
            //_connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<IEnumerable<ArticleItemSummaryVM>> GetSummaryArticleItemsAsync(string subjectId)
        {
            //return new Task<IEnumerable<ArticleItemSummaryVM>>(() => new List<ArticleItemSummaryVM>() {
            //new ArticleItemSummaryVM ()
            //{
            //    ArxivId ="aaa",
            //    PrimarySubject =new SubjectItemVM (){ Code ="aa", Description="begin start" },
            //    Title="begin"}
            //});

            var test =  new List<ArticleItemSummaryVM>() {
            new ArticleItemSummaryVM ()
            {
                ArxivId ="aaa",
                PrimarySubject =new SubjectItemVM (){ Code ="aa", Description="begin start" },
                Title="begin"}
            };

            return await Task.FromResult(test);
        }
    }
}
