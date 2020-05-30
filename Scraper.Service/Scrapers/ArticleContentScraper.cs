using HtmlAgilityPack;
using Scraper.Service.Util;
using Scraper.Types.Service;
using System.Threading;
using System.Threading.Tasks;

namespace Scraper.Service.Scrapers
{
    public interface IArticleContentScraper
    {
        Task<ArticleItemDto> GetArticle(string url, CancellationToken cancellationToken);
    }

    public class ArticleContentScraper : IArticleContentScraper
    {
        public async Task<ArticleItemDto> GetArticle(string url, CancellationToken cancellationToken)
        {
            ArticleItemDto dto = new ArticleItemDto();

            HtmlDocument doc = await HtmlAgilityHelper.GetHtmlDocument(url, cancellationToken);

            var raw = doc.ToString();

            //TODO : 

            return dto;
        }

        #region Private



        #endregion
    }
}
