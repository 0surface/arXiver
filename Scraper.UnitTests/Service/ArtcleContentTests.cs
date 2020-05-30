using HtmlAgilityPack;
using Scraper.Service.Scrapers;
using Xunit;
using static Scraper.UnitTests.Service.Builders;

namespace Scraper.UnitTests.Service
{
    public class ArtcleContentTests : ArticleContentTestBuilder
    {
        public ArticleContentTestBuilder _testBuilder = new ArticleContentTestBuilder();
        IArticleContentScraper _scraper = new ArticleContentScraper();

        [Fact]
        public void Scrapes_Article_Title_From_Html()
        {
            //Arrange
            HtmlDocument doc = _testBuilder.ArticleContent_1_HtmlDocument();
            var expected = _testBuilder.Article_1_Title;

            //Act
            //TODO: add test
            //var sut = _scraper.GetArticle()

            //Assert
            Assert.Equal(1, 1);

        }
    }
}
