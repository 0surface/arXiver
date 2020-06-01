using Scraper.Domain.SeedWork;
using System.Collections.Generic;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class Author
        : Entity

    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public List<AuthorArticle> AuthorArticles { get; set; }

        protected Author()
        {
            AuthorArticles = new List<AuthorArticle>();
        }

        public Author(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
