using Scraper.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Domain.AggregatesModel.ScrapeAggregate
{
    public class ScrapeJob
        : Entity, IAggregateRoot
    {
        public Guid ScrapeRay { get; set; }
        public string IssuedBy { get; set; }
        public bool IsSuccess { get; set; }

        private readonly List<ScrapeJobItem> _scrapeJobItems;
        public IReadOnlyCollection<ScrapeJobItem> ScrapeJobItems => ScrapeJobItems;

        public DateTime CreatedDate { get; private set; }
        public DateTime JobStartDate { get; private set; }
        public DateTime JobCompletedDate { get; set; }

        protected ScrapeJob()
        {
            _scrapeJobItems = new List<ScrapeJobItem>();
        }

        public void AddScrapeJobItem()
        {

        }

        public void SetJobStartDate(DateTime date)
        {
            JobStartDate = date;
        }

        public void SetJobCompletedDate(DateTime date)
        {
            JobStartDate = date;
        }
    }
}
