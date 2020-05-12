using Scraper.Domain.SeedWork;
using System.Collections.Generic;

namespace Scraper.Domain.AggregatesModel.SubjectAggregate
{
    public class Subject
        : Entity
    {
        public string ArxivId { get; private set; }
        public string FullName { get; private set; }
        public string Discipline { get; private set; }
        public string DisciplineId { get; private set; }

        private List<string> _subCatagories;
        public IReadOnlyCollection<string> SubCatagories => _subCatagories;
    }
}
