using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scraper.API.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.API.Infrastructure
{
    public class ArticleItemContext  : DbContext
    {
        public DbSet<ArticleItemEntity> ArticleItems { get; set; }
        public DbSet<SubjectItemEntity> SubjectItems { get; set; }
        public DbSet<AuthorEntity> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration["ConnectionString"];
            optionsBuilder.UseSqlServer(connectionString);
        }

    }
}
