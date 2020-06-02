using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class AuthorArticle :Entity
    {
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public int ArticleId { get; set; }        
        public Article Article { get; set; }
    }

}
