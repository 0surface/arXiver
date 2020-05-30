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
        public Discipline Discipline { get; private set; }

        private List<string> _categories;
        public IReadOnlyCollection<string> Categories => _categories;

        protected Subject() 
        {
            _categories = new List<string>();
            Discipline = null;
        }

        public Subject(string subjectCode, string name)
        {
            Code = subjectCode;
            Name = name;
        }

        public void AddCategories(string catagory)
        {            
            if(_categories.Exists(s => s.Contains(catagory) == false))
            {
                _categories.Add(catagory);
            }
        }

        public void AddDiscipline(string code)
        {
            if (!string.IsNullOrEmpty(code) && Discipline == null)
            {
                var matching = Enumeration.GetAll<Discipline>().Where(x => x.Code == code).FirstOrDefault();
                if(matching != null)
                {
                    Discipline = matching;
                }
            }
        }
    }
}
