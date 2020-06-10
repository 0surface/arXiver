using Scraper.Domain.AggregatesModel.ArticleAggregate;
using System;
using System.Collections.Generic;

namespace Scraper.API.Infrastructure.Repositories
{
    public interface IArticleItemRepository
    {
        int SaveBySubjectGroup(List<Article> articles);
    }

    public class ArticleItemRepository : IArticleItemRepository
    {
        private IArticleContext _context;

        public ArticleItemRepository()
        {
            _context = new ArticleContext();
        }

        public ArticleItemRepository(IArticleContext context)
        {
            _context = context;
        }

        public int SaveBySubjectGroup(List<Article> articles)
        {
            try
            {
                _context.Articles.AddRange(articles);

                return articles.Count > 0 ? _context.SaveChanges() : 0;
            }
            catch (Exception ex)
            {
                var c = ex.Message;
                return 0;
            }
        }

    }
}
