namespace Scraper.Types.Service
{
    public class VersionDto
    {
        public string VersionId { get; set; }
        public string HtmlLink { get; set; }
        public string SubmissionDate { get; set; }
        public string Tag { get; set; }
        public string CitationSubjectCode { get; set; }
        public int SizeInKiloBytes { get; set; }
        public bool IsLatest { get; set; }
    }
}
