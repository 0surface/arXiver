using Scraper.Domain.SeedWork;
using System.Collections.Generic;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectItem : ValueObject
    {
        public string SujectCode { get; private set; }
        public string Name { get; private set; }
        public bool IsMainSubject { get; private set; }

        public SubjectItem() { }

        public SubjectItem(string arxivId, string name, bool isMainSubject)
        {
            SujectCode = arxivId;
            Name = name;
            IsMainSubject = isMainSubject;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return SujectCode;
            yield return Name;
            yield return IsMainSubject;
        }
    }
}
