using Scraper.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Domain.AggregatesModel.ScrapeAggregate
{
    public class ScrapeJobItem 
        : Entity, IAggregateRoot
    {
        public string Url { get; set; }
        public bool IsSuccess { get; set; }

        public DateTime CreatedDate { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDateDate { get; private set; }

        protected ScrapeJobItem()
        {
            CreatedDate = DateTime.Now;
        }

        public ScrapeJobItem(string url)
        {

        }
    }
}
