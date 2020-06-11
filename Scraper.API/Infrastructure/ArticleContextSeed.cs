using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Scraper.API.Extensions;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Domain.AggregatesModel.SubjectAggregate;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.API.Infrastructure
{
    public class ArticleContextSeed
    {
        /*For a comprehensive look at the construction of the methodology & desgin of DbCOntext Seeding
         * Go to https://github.com/dotnet-architecture/eShopOnContainers and view the Ordering Microserice API.
         */

        public async Task SeedAsync(ArticleContext context, IWebHostEnvironment env, IOptions<ScraperSettings> settings, ILogger<ArticleContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(ArticleContextSeed));
            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = settings.Value
                .UseCustomizationData;

                var contentRootPath = env.ContentRootPath;

                using (context)
                {
                    context.Database.Migrate();

                    //Subject Aggregate Seeding
                    if (!context.Subjects.Any())
                    {
                        context.Subjects.AddRange(useCustomizationData
                            ? GetPredefinedSubjects()
                            : GetSubjectsFromFile(contentRootPath, logger));
                    }

                    //TODO : Article Aggregate Seeding
                    if (!context.Articles.Any())
                    {
                        context.Articles.AddRange(useCustomizationData
                            ? GetArticlesFromFile(contentRootPath, logger)
                            : GetPredefinedArticles());
                    }

                    if (!context.SubjectItems.Any())
                    {
                        context.SubjectItems.AddRange(useCustomizationData
                            ? GetPredefinedSubjectItems()
                            : GetSubjectItemsFromFile(contentRootPath, logger));
                    }

                    await context.SaveChangesAsync();
                }
            });
        }

        #region Article Aggregate

        private IEnumerable<SubjectItem> GetPredefinedSubjectItems()
        {
            return new List<SubjectItem>()
           {
               new SubjectItem ("astro-ph.GA", "Astrophysics of Galaxies", true),
               new SubjectItem ("astro-ph.GA", "Astrophysics of Galaxies", false)
           };
        }

        private IEnumerable<SubjectItem> GetSubjectItemsFromFile(string contentRootPath, ILogger<ArticleContextSeed> logger)
        {
            List<SubjectItem> subjectItems = new List<SubjectItem>();

            foreach (var subject in GetSubjectsFromFile(contentRootPath, logger))
            {
                subjectItems.Add(new SubjectItem(subject.Code, subject.Name, true));
                subjectItems.Add(new SubjectItem(subject.Code, subject.Name, false));
            }

            return subjectItems;
        }

        private IEnumerable<Article> GetPredefinedArticles()
        {
            return new List<Article>();
        }

        private IEnumerable<Article> GetArticlesFromFile(string contentRootPath, ILogger<ArticleContextSeed> log)
        {
            string csvArticles = Path.Combine(contentRootPath, "Setup", "Articles.csv");

            if (!File.Exists(csvArticles))
            {
                return GetPredefinedArticles();
            }
            //TODO: Fetch, read and return Articles from setup file
            return new List<Article>();
        }

        #endregion Article Aggregate

        #region Subjects Aggregate

        private IEnumerable<Subject> GetSubjectsFromFile(string contentRootPath, ILogger<ArticleContextSeed> log)
        {
            string csvSubjects = Path.Combine(contentRootPath, "Setup", "Subjects.csv");

            if (!File.Exists(csvSubjects))
            {
                return GetPredefinedSubjects();
            }

            return File.ReadAllLines(csvSubjects)
                        .SelectTry(line => CreateSubject(line))
                        .OnCaughtException(ex => { log.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                        .Where(x => x != null);
        }

        private Subject CreateSubject(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception("Subject is null or empty");
            }

            var splitText = value.Split(',');
            if (splitText.Length != 5)
            {
                throw new Exception("Subject is Invalid or empty");
            }

            Subject subject = new Subject(splitText[0], splitText[1]
                                           , splitText[2], splitText[3], splitText[4]);
            return subject;
        }

        private IEnumerable<Subject> GetPredefinedSubjects()
        {
            return new List<Subject>()
            {
                new Subject("astro-ph.GA", "Astrophysics of Galaxies", "Astrophysics", "astro-ph", "Physics")
            };
        }

        #endregion Subjects Aggregate

        private AsyncRetryPolicy CreatePolicy(ILogger<ArticleContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}

