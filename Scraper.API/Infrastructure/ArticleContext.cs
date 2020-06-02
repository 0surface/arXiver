using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Domain.AggregatesModel.SubjectAggregate;
using System.IO;

namespace Scraper.API.Infrastructure
{
    public class ArticleContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Version> Versions { get; set; }      
        public DbSet<AuthorArticle> AuthorArticles { get; set; }
        public DbSet<SubjectItemArticle> SubjectItemArticles { get; set; }

        public DbSet<ScientificField> ScientificFields { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Subject> Subjects { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            /* This helps EF core understand the intended database schema, 
             * setting up a Many-To-Many relationship between Article and author */
            modelBuilder.Entity<AuthorArticle>().HasKey(s => new { s.AuthorId, s.ArticleId });
            modelBuilder.Entity<SubjectItemArticle>().HasKey(s => new { s.SubjectItemId, s.ArticleId });

            modelBuilder.Entity<Author>().ToTable("Authors");
            modelBuilder.Entity<SubjectItem>().ToTable("SubjectItem");
        }

    }
}

