using Scraper.Domain.SeedWork;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Domain.AggregatesModel.SubjectAggregate
{
    public class Subject
        : Entity, IAggregateRoot
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string GroupCode { get; private set; }
        public string GroupName { get; private set; }        
        public string Discipline { get; private set; }


        private List<string> _categories;
        public IReadOnlyCollection<string> Categories => _categories;

        protected Subject() 
        {
            _categories = new List<string>();
        }

        public Subject(string subjectCode, string name
            ,string groupName, string groupCode, string discipline)
        {
            Code = subjectCode;
            Name = name;
            GroupName = groupName;
            GroupCode = groupCode;
            Discipline = discipline;
            _categories = new List<string>();
        }

        public void AddCategories(string catagory)
        {            
            if(_categories.Exists(s => s.Contains(catagory) == false))
            {
                _categories.Add(catagory);
            }
        }
    }
}

