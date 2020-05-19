using System.Collections.Generic;

namespace Scraper.Types.Service
{
    public class ArticleScrapedDataDto
    {
        public string DisplayDate { get; set; }
        public string ArxivId { get; set; }
        public string AbstractUrl { get; set; }
        public string PdfUrl { get; set; }
        public string OtherFormatUrl { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string AbstractText { get; set; }
        public string[] PrimarySubject { get; set; }
        public List<ScrapedArticleListSubjectDto> Subjects { get; set; } = new List<ScrapedArticleListSubjectDto>();
        public List<ScrapedAuthorDto> Authors { get; set; } = new List<ScrapedAuthorDto>();
    }

    public class ScrapedAuthorDto
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public string ContextUrl { get; set; }
    }

    public class ScrapedArticleListSubjectDto 
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

}
