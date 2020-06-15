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

            var result = await _scrapeCommandService.SubmissionsBySubjectCodeAsync(newSubmissionsUrl, subjectCode, cancellationToken);

            if (result.Count > 0)
                return Ok();
            else if (result.IsSucess)
                return NoContent();
            else                
                return StatusCode((int)HttpStatusCode.InternalServerError);
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

            var result = await _scrapeCommandService.SubmissionsBySubjectGroupAsync(newSubmissionsUrl, subjectGroup, cancellationToken);

            if (result.Count > 0)
                return Ok();
            else if (result.IsSucess)
                return NoContent();
            else
                return StatusCode((int)HttpStatusCode.InternalServerError);
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

            var result = await _scrapeCommandService.CatchupBySubjectGroupAsync(catchUpUrl, cancellationToken);

            if (result.Count > 0)
            {
                return Content($"{baseUrl}{result.ContinueUrl}");
            }
            else if (result.IsSucess)
            {
                return NoContent();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}


