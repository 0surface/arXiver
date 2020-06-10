using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper.API.Application.Queries;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleItemSummaryVM>>> GetSummaryArticleItemsAsync(string subjectId)
        {
            var summary = await _articleQueries.GetSummaryArticleItemsAsync(subjectId);
            return Ok(summary);
        }
    }
}
