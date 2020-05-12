using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class Author
        : Entity

    {
        public string AuthorId { get; private set; }
        public string FullName { get; private set; }

        public Author(string fullName, string authorId)
        {
            FullName = fullName;
            AuthorId = authorId;
        }
    }
}
