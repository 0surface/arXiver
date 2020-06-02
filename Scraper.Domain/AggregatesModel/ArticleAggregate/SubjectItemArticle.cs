using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectItemArticle : Entity
    {
        public int SubjectItemId { get; set; }
        public SubjectItem SubjectItem { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
