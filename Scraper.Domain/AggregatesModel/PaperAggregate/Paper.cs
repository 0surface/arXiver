using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.PaperAggregate
{
    public class Paper
        : Entity
    {
        public string IdString { get; private set; }
        public string Link { get; private set; }
    }
}
