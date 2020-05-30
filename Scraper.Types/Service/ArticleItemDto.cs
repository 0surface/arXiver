using System.Collections.Generic;

namespace Scraper.Types.Service
{
    public class ArticleItemDto
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
        public List<SubjectItemDto> Subjects { get; set; } = new List<SubjectItemDto>();
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    }

  

   
}
