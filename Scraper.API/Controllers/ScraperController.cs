using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.API.Infrastructure.Mapping;
using Scraper.API.Infrastructure.Repositories;
using Scraper.Domain.AggregatesModel.ArticleAggregate;
using Scraper.Service.Scrapers;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ScraperController : Controller
    {
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleListScraper _articleListScraper;
        private readonly IScrapeMapper _scrapeMapper;

        private readonly IArticleItemRepository _repo;
        private readonly string baseUrl = "http://export.arxiv.org"; //TODO : Fetch from settings json file


        public ScraperController(IArticleListScraper articleListScraper,
            IScrapeMapper scrapeMapper,
            IArticleItemRepository repo,
            ILogger<ArticlesController> logger)
        {
            _articleListScraper = articleListScraper;
            _scrapeMapper = scrapeMapper;
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("SubjectGroupNewSubmission")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ScrapeBySubjectGroupNew(string subjectGroup, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(subjectGroup) || string.IsNullOrEmpty(subjectGroup) )
                return BadRequest();

            string newSubmissionsUrl = $"{baseUrl}/list/{subjectGroup}/new";

            var dtoList = await _articleListScraper.GetArticles(newSubmissionsUrl, true, true, cancellationToken);

            var articles = _scrapeMapper.MapArticleToDomain(dtoList);

            if (articles == null)
                return NoContent();

            var newSubmissions = articles
                .Where(a => a.ScrapeContext == ArticleScrapeContextEnum.Submission)
                ?.ToList();

            return _repo.SaveBySubjectGroup(newSubmissions) == 0 ?
                 StatusCode((int)HttpStatusCode.InternalServerError)
                 : Ok();            
        }

        [HttpPost("SubjectGroupRecent")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ScrapeBySubjectGroupRecent(string subjectGroup, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subjectGroup) || string.IsNullOrEmpty(subjectGroup))
            {
                return BadRequest();
            }

            string url = $"{baseUrl}/list/{subjectGroup}/recent";

            var dtoList = await _articleListScraper.GetArticles(url, false, true, cancellationToken);
            
            var articles = _scrapeMapper.MapArticleToDomain(dtoList);

            if (articles == null)
                return NoContent();

            return _repo.SaveBySubjectGroup(articles) == 0 ?
                 StatusCode((int)HttpStatusCode.InternalServerError)
                 : Ok();
        }        
    }
}


