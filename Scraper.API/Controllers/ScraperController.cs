using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.API.Infrastructure.Repositories;
using Scraper.Service.Scrapers;
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
        private readonly IArticleItemRepository _repo;
        private readonly string baseUrl = "http://export.arxiv.org"; //TODO : Fetch from settings json file


        public ScraperController(IArticleListScraper articleListScraper,
            IArticleItemRepository repo,
            ILogger<ArticlesController> logger)
        {
            _articleListScraper = articleListScraper;
            _repo = repo;
            _logger = logger;
        }

        [HttpPost("SubjectGroupNew")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ScrapeBySubjectGroupNew(string subjectGroup, CancellationToken cancellationToken)
        {
            if(string.IsNullOrEmpty(subjectGroup) || string.IsNullOrEmpty(subjectGroup) )
            {
                return BadRequest();
            }

            string url = $"{baseUrl}/list/{subjectGroup}/new";

            var articles = await _articleListScraper.GetArticles(url, true, cancellationToken);

            if (articles == null)
                return NoContent();

            return _repo.SaveBySubjectGroup(articles) == 0 ?
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

            var articles = await _articleListScraper.GetArticles(url, false, cancellationToken);
            if (articles == null)
                return NoContent();

            return _repo.SaveBySubjectGroup(articles) == 0 ?
                 StatusCode((int)HttpStatusCode.InternalServerError)
                 : Ok();
        }        
    }
}


