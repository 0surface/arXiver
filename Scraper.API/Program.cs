using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scraper.API.Infrastructure;
using System;
using System.IO;

namespace Scraper.API
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            try
            {
                var host = BuildWebHost(configuration, args);

                host.MigrateDbContext<ArticleContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();
                    var settings = services.GetService<IOptions<ScraperSettings>>();
                    var logger = services.GetService<ILogger<ArticleContextSeed>>();

                    new ArticleContextSeed()
                        .SeedAsync(context, env, settings, logger)
                        .Wait();
                });

                host.Run();

                return 0;
            }
            catch (Exception)
            {
                //Log
                return 1;
            }
            finally
            {
                //Log
            }
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .Build();

        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            //TOOD : Optional : assAzure key vault.

            return builder.Build();
        }

        private static int GetDefinedPorts(IConfiguration config)
        {
            var httpPort = config.GetValue("PORT", 80);
            return httpPort;
        }
    }
}
