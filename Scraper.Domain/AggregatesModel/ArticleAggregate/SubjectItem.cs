using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectItem
        : Entity
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public bool IsPrimary { get; private set; }

        public SubjectItem(string code, string name, bool isPrimary)
        {
            Code = code;
            Name = name;
            IsPrimary = isPrimary;
        }
    }
}
