using Scraper.Domain.SeedWork;
using System.Collections.Generic;

namespace Scraper.Domain.AggregatesModel.SubjectAggregate
{
    public class Subject
        : Entity, IAggregateRoot
    {
        public string SubjectCode { get; private set; }
        public string FullName { get; private set; }
        public string DisciplineName { get; private set; }
        public string DisciplineCode { get; private set; }
        public string Field  {get; private set; }

        private List<string> _categories;
        public IReadOnlyCollection<string> Categories => _categories;

        protected Subject() 
        {
            _categories = new List<string>();
        }

        public Subject(string subjectCode, string name, string disciplineName
            , string disciplineCode, string field)
        {
            SubjectCode = subjectCode;
            FullName = name;
            DisciplineName = disciplineName;
            DisciplineCode = disciplineCode;
            Field = field;
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
