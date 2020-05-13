using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectItem
        : Entity
    {
        public string SubjectCode { get; private set; }
        public string Name { get; private set; }
        public bool IsPrimarySubject { get; private set; }

        public SubjectItem(string subjectCode, string name, bool isPrimarySubject)
        {
            SubjectCode = subjectCode;
            Name = name;
            IsPrimarySubject = isPrimarySubject;
        }
    }
}
