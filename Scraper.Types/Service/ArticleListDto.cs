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
        public string[] PrimarySubject { get; set; }
        public Dictionary<string, string> Subjects { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Authors { get; set; } = new Dictionary<string, string>();
    }
}
