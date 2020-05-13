using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectItem
        : Entity
    {
        public string SubjectCode { get; private set; }
        public string Name { get; private set; }
        public bool IsMainSubject { get; private set; }

        public SubjectItem(string subjectCode, string name, bool isMainSubject)
        {
            SubjectCode = subjectCode;
            Name = name;
            IsMainSubject = isMainSubject;
        }
    }
}
