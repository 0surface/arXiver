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
            if (string.IsNullOrEmpty(subjectCode)) return BadRequest();

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
            if (string.IsNullOrEmpty(subjectGroup)) return BadRequest();

            string newSubmissionsUrl = $"{baseUrl}/list/{subjectGroup}/new";

            int result = await _scrapeCommandService.SubmissionsBySubjectGroupAsync(newSubmissionsUrl, subjectGroup, cancellationToken);

            return result >= 1 ? Ok() :
                  (result == 0) ? NoContent() : StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost("catchupbygroup")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CatchUpBySubjectGroup(string subjectGroup, string startDay, string startMonth,
                                                  string startYear, string returnAmount, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subjectGroup))
                return BadRequest();

            string catchUpUrl = $"{baseUrl}/catchup?smonth={startMonth}&group=grp_&sday={startDay}&num={returnAmount}&archive={subjectGroup}&method=with&syear={startYear}";

            (int, string) repoResult_ContinueUrl =
                await _scrapeCommandService.CatchupBySubjectGroupAsync(catchUpUrl, cancellationToken);

            int repoResult = repoResult_ContinueUrl.Item1;

            string continueUrl = $"{baseUrl}{repoResult_ContinueUrl.Item2}";

            if (repoResult >= 1)
                return Content(continueUrl);

            return (repoResult == 0) ?
                  NoContent()
                : StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}


