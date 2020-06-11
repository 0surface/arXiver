using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.API.Infrastructure.Services;
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
        private readonly IScrapeCommandService _scrapeCommandService;

        private readonly string baseUrl = "http://export.arxiv.org"; //TODO : Fetch from settings json file


        public ScraperController(IScrapeCommandService scrapeCommandService,
            ILogger<ArticlesController> logger)
        {
            _scrapeCommandService = scrapeCommandService;
            _logger = logger;
        }

        [HttpPost("submissionsbysubject")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> NewBySubjectCode(string subjectCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subjectCode) || string.IsNullOrEmpty(subjectCode))
                return BadRequest();

            string newSubmissionsUrl = $"{baseUrl}/list/{subjectCode}/new";

            int result = await _scrapeCommandService.SubmissionsBySubjectCodeAsync(newSubmissionsUrl, subjectCode, cancellationToken);

            return result >= 1 ? Ok() :
                  (result == 0) ? NoContent() : StatusCode((int)HttpStatusCode.InternalServerError);
        }



        [HttpPost("submissionsbygroup")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> NewBySubjectGroup(string subjectGroup, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subjectGroup) || string.IsNullOrEmpty(subjectGroup))
                return BadRequest();

            string newSubmissionsUrl = $"{baseUrl}/list/{subjectGroup}/new";

            int result  = await _scrapeCommandService.SubmissionsBySubjectGroupAsync(newSubmissionsUrl, subjectGroup, cancellationToken);

            return result >= 1 ? Ok() :
                  (result == 0) ? NoContent() : StatusCode((int)HttpStatusCode.InternalServerError);
        }

    }
}


