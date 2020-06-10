using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Domain.AggregatesModel.SubjectAggregate;
using Scraper.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class Article
        : Entity, IAggregateRoot
    {
        public string ArxivId { get; private set; }
        public string HtmlLink { get; private set; }
        public string PdfUrl { get; set; }
        public string OtherFormatUrl { get; set; }
        public string Title { get; private set; }
        public string AbstractText { get; private set; }
        public string Comments { get; private set; }
        public string JournalReference { get; private set; }
        public string JournalReferenceHtmlLink { get; private set; }
        public DateTime DisplayDate { get; private set; }
        public DateTime ScrapedDate { get; private set; }

        public List<AuthorArticle> AuthorArticles { get; private set; }
        public List<SubjectItemArticle> SubjectItemArticles { get; private set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so Versions cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method ArticleAggrergateRoot.AddVersion() which includes behaviour.
        private readonly List<Version> _versions;
        public IReadOnlyCollection<Version> Versions => _versions;

        public ArticleScrapeContextEnum ScrapeContext { get; private set; }

        protected Article()
        {
            AuthorArticles = new List<AuthorArticle>();
            SubjectItemArticles = new List<SubjectItemArticle>();
            _versions = new List<Version>();
            ScrapeContext = ArticleScrapeContextEnum.None;
        }

        public Article(string arxivId, string htmlLink, string pdfUrl, string otherFormatUrl,
            string title, string abstractText, string comments
            , string journalReference, string journalReferenceHtmlLink, DateTime scrapedDate)
        {
            ArxivId = arxivId;
            HtmlLink = htmlLink;
            PdfUrl = pdfUrl;
            OtherFormatUrl = otherFormatUrl;
            Title = title;
            AbstractText = abstractText;
            Comments = comments;
            JournalReference = journalReference;
            JournalReferenceHtmlLink = journalReferenceHtmlLink;
            ScrapedDate = scrapedDate;

            AuthorArticles = new List<AuthorArticle>();
            SubjectItemArticles = new List<SubjectItemArticle>();
            _versions = new List<Version>();
            ScrapeContext = ArticleScrapeContextEnum.None;
        }

        public void AddAuthor(string name, string code)
        {
            if (AuthorArticles.Exists(a => a.Author.Code == code) == false)
            {
                var author = new Author(name, code);
                AuthorArticles.Add(new AuthorArticle() { Author = author, Article = this });
            }
        }

        public void AddVersion(string arxivId, string htmlLink, DateTime submissionDate, string tag
            , string citationSubjectCode, int sizeInKiloBytes)
        {

            if (_versions == null || _versions.Count == 0)
            {
                var version = new Version(arxivId, htmlLink, submissionDate, tag, citationSubjectCode, sizeInKiloBytes);
                _versions.Add(version);
            }
            else
            {
                var existingVersionForThisPaper = _versions.Where(v => v.VersionedId == arxivId).SingleOrDefault();

                if (existingVersionForThisPaper == null)
                {
                    var version = new Version(arxivId, htmlLink, submissionDate, tag, citationSubjectCode, sizeInKiloBytes);
                    _versions.Add(version);
                }
            }
        }

        public void AddPrimarySubject(string code, string name)
        {
            AddSubjectItem(code, name, true);
        }

        public void AddSubjectItem(string code, string name, bool isPrimary = false)
        {
            //if (SubjectItemArticles.Exists(sa => sa.SubjectItem.Code == code) == false)
            //{
                var item = new SubjectItemArticle()
                {
                    Article = this,
                    // SubjectItem = new SubjectItem(code, name, isPrimary)
                };

                SubjectItemArticles.Add(item);
            //}
        }

        public void AddExistingSubjectItem(SubjectItem subjectItem)
        {
            SubjectItemArticles.Add(new SubjectItemArticle()
            {
                Article = this,
                SubjectItemId =subjectItem.Id
                //SubjectItem = subjectItem
            });
        }       

        //public void AddSubject(string subjectCode, string name, bool isPrimary)
        //{
        //    var existingSubjectForPaper = _subjectItems.Where(s => s.Code == subjectCode).SingleOrDefault();

        //    if(existingSubjectForPaper == null)
        //    {
        //        _subjectItems.Add(new SubjectItem(subjectCode, name, isPrimary));
        //    }
        //}

        public void AddDisplayDate(string input)
        {
            DisplayDate = DateTime.TryParse(input, out DateTime date)
                ? date
                : DateTime.MinValue;
        }

        public void AddScrapeContext(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.ToLower().Contains("submission"))
                    ScrapeContext = ArticleScrapeContextEnum.Submission;
                else if (input.ToLower().Contains("cross-lists") || input.ToLower().Contains("cross"))
                    ScrapeContext = ArticleScrapeContextEnum.CrossList;
                else if (input.ToLower().Contains("Replacement"))
                    ScrapeContext = ArticleScrapeContextEnum.Replacement;
            }
        }
    }
}
