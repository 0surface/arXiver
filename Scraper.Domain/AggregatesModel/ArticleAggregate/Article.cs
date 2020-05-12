using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Domain.AggregatesModel.PaperAggregate
{
    public class Article
        : Entity, IAggregateRoot
    {
        public string ArxivId { get; private set; }
        public string HtmlLink { get; private set; }
        public string Title { get; private set; }
        public string AbstractText { get; private set; }
        public string Comments { get; private set; }
        public string JournalReference { get; private set; }
        public string JournalReferenceHtmlLink { get; private set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so Authors cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method PaperAggrergateRoot.AddAuthor() which includes behaviour.
        private readonly List<Author> _authors;
        public IReadOnlyCollection<Author> Authors => _authors;

        private readonly List<Version> _versions;
        public IReadOnlyCollection<Version> Versions => _versions;

        private List<SubjectItem> _subjects;
        public IReadOnlyCollection<SubjectItem> Subjects => _subjects;


        protected Article()
        {
            _authors = new List<Author>();
            _versions = new List<Version>();
            _subjects = new List<SubjectItem>();
        }

        public Article(string arxivId, string htmlLink, string title, string abstractText, string comments, string subjects,
            string journalReference, string journalReferenceHtmlLink)
        {
            ArxivId = arxivId;
            HtmlLink = htmlLink;
            Title = title;
            AbstractText = abstractText;
            Comments = comments;
            JournalReference = journalReference;
            JournalReferenceHtmlLink = journalReferenceHtmlLink;
        }

        public void AddAuthor(string fullName, string authorId)
        {
            var existingAuthorForPaper = _authors.Where(a => a.AuthorId == authorId).SingleOrDefault();

            if(existingAuthorForPaper == null)
            {
                var author = new Author(fullName, authorId);
                _authors.Add(author);
            }
        }

        public void AddVersion(string arxivId, string htmlLink, DateTime submissionDate, string tag
            , string citationSubjectCode, int sizeInKiloBytes, bool isLatest = true)
        {

            if (_versions == null || _versions.Count == 0)
            {
                var version = new Version(arxivId, htmlLink, submissionDate, tag, citationSubjectCode, sizeInKiloBytes, isLatest);
                _versions.Add(version);
            }
            else
            {
                var existingVersionForThisPaper = _versions.Where(v => v.ArxivId == arxivId).SingleOrDefault();

                if (existingVersionForThisPaper == null)
                {
                    var version = new Version(arxivId, htmlLink, submissionDate, tag, citationSubjectCode, sizeInKiloBytes, isLatest);
                    _versions.Add(version);
                }
            }
        }

        public void AddSubject(string subjectCode, string name, bool isMainSubject)
        {
            var existingSubjectForPaper = _subjects.Where(s => s.SubjectCode == subjectCode).SingleOrDefault();

            if(existingSubjectForPaper == null)
            {
                _subjects.Add(new SubjectItem(subjectCode, name, isMainSubject));
            }

        }
    }
}
