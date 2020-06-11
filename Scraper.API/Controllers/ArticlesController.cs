using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.API.Application.Queries;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Scraper.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticleQueries _articleQueries;

        public ArticlesController(IArticleQueries articleQueries,
            ILogger<ArticlesController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _articleQueries = articleQueries ?? throw new ArgumentNullException(nameof(articleQueries));
        }

        [HttpGet("summary")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ArticleItemSummaryVM>>> GetSummaryArticlesAsync(string subjectCode)
        {
            var summary = await _articleQueries.GetSummaryArticlesAsync(subjectCode);

            if (summary != null)
                return Ok(summary);
            else
                return NotFound();
        }


        [HttpGet("full")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ArticleItemVM>>> GetArticlesAsync(string subjectCode)
        {
            var summary = await _articleQueries.GetArticlesAsync(subjectCode);

            if (summary != null)
                return Ok(summary);
            else
                return NotFound();
        }
    }
}
