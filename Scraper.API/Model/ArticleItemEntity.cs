using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.API.Model
{
    public class ArticleItemEntity
    {
        public int Id { get; set; }
        public string DisplayDate { get; set; }
        public string ArxivId { get; set; }
        public string AbstractUrl { get; set; }
        public string PdfUrl { get; set; }
        public string OtherFormatUrl { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string AbstractText { get; set; }
    }
}
