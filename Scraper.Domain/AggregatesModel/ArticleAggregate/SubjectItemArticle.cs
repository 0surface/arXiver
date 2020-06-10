using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectItemArticle
    {   
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public int SubjectItemId { get; set; }
    }
}
