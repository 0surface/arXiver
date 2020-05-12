using Microsoft.EntityFrameworkCore;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Domain.AggregatesModel.SubjectAggregate;

namespace Scraper.Infrastructure
{
    public class ScraperContext : DbContext //, IUnitOfWork
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Subject> Subjects { get; set; }

    }
}
