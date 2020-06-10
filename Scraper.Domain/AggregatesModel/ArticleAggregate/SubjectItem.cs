using Scraper.Domain.SeedWork;
using System.Collections.Generic;

namespace Scraper.Domain.AggregatesModel.ArticleAggregate
{
    public class SubjectItem
        : Entity
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public bool IsPrimary { get; private set; }
        //public List<SubjectItemArticle> SubjectItemArticles { get; set; }
        
        protected SubjectItem()
        {
            //SubjectItemArticles = new List<SubjectItemArticle>();
        }

        public SubjectItem(string code, string name, bool isPrimary)
        {
            Code = code;
            Name = name;
            IsPrimary = isPrimary;
        }

        public bool SubjectIsEmpty()
        {
            return this == null || string.IsNullOrEmpty(Code);
        }
    }
}
