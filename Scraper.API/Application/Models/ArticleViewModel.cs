using System.Collections.Generic;

namespace Scraper.API.Application.Queries
{
    public class ArticleItemVM
    {
        public string DisplayDate { get; set; }
        public string ArxivId { get; set; }
        public string AbstractUrl { get; set; }
        public string PdfUrl { get; set; }
        public string OtherFormatUrl { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string AbstractText { get; set; }
        public SubjectItemVM PrimarySubject { get; set; }
        public List<SubjectItemVM> Subjects { get; set; } = new List<SubjectItemVM>();
        public List<AuthorVM> Authors { get; set; } = new List<AuthorVM>();
    }

    public class ArticleItemSummaryVM
    {
        public string ArxivId { get; set; }
        public string Title { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
    }

    public class SubjectItemVM
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class AuthorVM
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public string ContextUrl { get; set; }
    }
}
