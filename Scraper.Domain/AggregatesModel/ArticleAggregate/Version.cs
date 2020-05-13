using Scraper.Domain.SeedWork;
using System;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class Version
        : Entity
    {
        public string VersionId { get; private set; }
        public string HtmlLink { get; private set; }
        public DateTime SubmissionDate { get; private set; }
        public string Tag { get; private set; }
        public string CitationSubjectCode { get; private set; }
        public int SizeInKiloBytes { get; private set; }
        public bool IsLatest { get; private set; }

        public Version(string versionId, string htmlLink, DateTime submissionDate
            , string tag, string citationSubjectCode, int sizeInKiloBytes, bool isLatest)
        {
            VersionId = versionId;
            HtmlLink = htmlLink;
            SubmissionDate = submissionDate;
            Tag = tag;
            CitationSubjectCode = citationSubjectCode;
            SizeInKiloBytes = sizeInKiloBytes;
            IsLatest = isLatest;
        }
    }
}


