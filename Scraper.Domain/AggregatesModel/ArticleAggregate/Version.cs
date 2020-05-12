using Scraper.Domain.SeedWork;
using System;
using System.Collections.Generic;

namespace Scraper.Domain.AggregatesModel.PaperAggregate
{
    public class Version : ValueObject
    {
        public string ArxivId { get; private set; }
        public string HtmlLink { get; private set; }
        public DateTime SubmissionDate { get; private set; }
        public string Tag { get; private set; }
        public string SubjectId { get; private set; }
        public bool IsLatest { get; private set; }        

        public Version() { }

        public Version(string arxivId, string htmlLink, DateTime submissionDate,  string tag, string subjectId, bool isLatest)
        {
            ArxivId = arxivId;
            HtmlLink = htmlLink;
            SubmissionDate = submissionDate;
            Tag = tag;
            SubjectId = subjectId;
            IsLatest = isLatest;            
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return ArxivId;
            yield return HtmlLink;
            yield return SubmissionDate;
            yield return Tag; 
            yield return SubjectId;
            yield return IsLatest;            
        }
    }
}

