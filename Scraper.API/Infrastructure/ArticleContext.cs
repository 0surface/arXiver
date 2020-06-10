using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Domain.AggregatesModel.SubjectAggregate;
using System.IO;

namespace Scraper.API.Infrastructure
{
    public interface IArticleContext
    {
         DbSet<Article> Articles { get; set; }
         DbSet<Version> Versions { get; set; }
         DbSet<AuthorArticle> AuthorArticles { get; set; }
         DbSet<SubjectItemArticle> SubjectItemArticles { get; set; }
         DbSet<Subject> Subjects { get; set; }
         DbSet<SubjectItem> SubjectItems { get; set; }

        int SaveChanges();
    }

    public class ArticleContext : DbContext, IArticleContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Version> Versions { get; set; }
        public DbSet<AuthorArticle> AuthorArticles { get; set; }
        public DbSet<SubjectItemArticle> SubjectItemArticles { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectItem> SubjectItems { get; set; }

        private IConfiguration Configuration { get; set; }

        public ArticleContext()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options) { }

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
            modelBuilder.Entity<SubjectItemArticle>().HasKey(s => new { s.ArticleId });

            modelBuilder.Entity<Author>().ToTable("Authors");
            modelBuilder.Entity<SubjectItem>().ToTable("SubjectItems");
        }
    }
}


