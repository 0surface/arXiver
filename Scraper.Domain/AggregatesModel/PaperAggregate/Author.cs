using Scraper.Domain.SeedWork;
using System.Collections.Generic;

namespace Scraper.Domain.AggregatesModel.PaperAggregate
{
    public class Author : ValueObject
    {
        public string FullName { get; private set; }
        public string AuthorId { get; private set; }

        public Author() { }

        public Author(string fullName, string authorId) 
        {
            FullName = fullName;
            AuthorId = authorId;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return FullName;
            yield return AuthorId;
        }
    }
}
