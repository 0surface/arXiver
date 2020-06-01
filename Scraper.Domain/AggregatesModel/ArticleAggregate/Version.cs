using Scraper.Domain.SeedWork;
using System;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class Version
        : Entity
    {
        public string VersionedId { get; private set; }
        public string HtmlLink { get; private set; }
        public DateTime SubmissionDate { get; private set; }
        public string Tag { get; private set; }
        public string CitationSubjectCode { get; private set; }
        public int SizeInKiloBytes { get; private set; }

        public Version(string versionedId, string htmlLink, DateTime submissionDate
            , string tag, string citationSubjectCode, int sizeInKiloBytes)
        {
            VersionedId = versionedId;
            HtmlLink = htmlLink;
            SubmissionDate = submissionDate;
            Tag = tag;
            CitationSubjectCode = citationSubjectCode;
            SizeInKiloBytes = sizeInKiloBytes;
        }
    }
}


