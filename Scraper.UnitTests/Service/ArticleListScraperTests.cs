using HtmlAgilityPack;
using Scraper.Service.Scrapers;
using System.Linq;
using Xunit;
using static Scraper.UnitTests.Service.Builders;

namespace Scraper.UnitTests.Service
{
    public class ArticleListScraperTests : ArticleListTestBuilder
    {
        public ArticleListTestBuilder _articleListTestBuilder = new ArticleListTestBuilder();
        IArticleListScraper _scraper = new ArticleListScraper(); //TODO: Can use Moq to inject repo, when implemented

        //[Fact]
        [Theory]
        [InlineData("//small", new string[] { "total of", "entries:" }, 63)] //Pass Scenario
        [InlineData("//small", null, 63)]//Pass Scenraio
        [InlineData("", new string[] { "total of", "entries:" }, 63)] //Pass Scenraio
        [InlineData("div[@id='content']", new string[] { "total of", "entries:" }, -1)]//Fail Scenraio        
        public void Gets_article_list_Entry_Count_in_htmlDoc(string articleListCountSelector, string[] identifierStrings, int expected)
        {
            //Arrange
            HtmlDocument doc = _articleListTestBuilder.ArticleList_withoutAbstract_HtmlDocument();

            //Act
            var sut = _scraper.GetArticleListEntryCount(doc, identifierStrings, articleListCountSelector);

            //Assert
            Assert.Equal(sut, expected);
        }

        [Fact]
        public void Gets_article_list_count_as_dto_collection()
        {
            //Arrange
            HtmlDocument doc = _articleListTestBuilder.ArticleList_withoutAbstract_HtmlDocument();
            int expected = 25;

            //Act
            var sut = _scraper.GetArticleList(doc, false).Count;

            //Assert
            Assert.Equal(sut, expected);
        }

        [Fact]
        public void Gets_article_abstract_text()
        {
            //Arrange
            HtmlDocument doc = _articleListTestBuilder.ArticleList_withAbstract_HtmlDocument();
            string expected = _articleListTestBuilder.WithAbstractHtml_ArticleItem2_AbstractText();

            //Act           
            var sut = _scraper.GetArticleList(doc, true)[1].AbstractText;

            //Assert
            Assert.Equal(sut, expected);
        }

        [Fact]
        public void Extracts_Articles_from_Catchup_Archive_HtmlDocument()
        {
            HtmlDocument doc = _articleListTestBuilder.ArticleList_Catchup_SubjectGroup_HtmlDocument();
            int expected = 299;

            var sut = _scraper.GetCatchUpArticleList(doc).Count();

            Assert.Equal(sut, expected);
        }

        [Fact]
        public void Labels_Context_Of_Catchup_Archive_Articles_As_Replaced()
        {
            HtmlDocument doc = _articleListTestBuilder.ArticleList_Catchup_SubjectGroup_HtmlDocument();
            int expected = 150;

            var sut = _scraper.GetCatchUpArticleList(doc)
                                 .Where(r => r.ArxivIdLabel == "replaced").Count();

            Assert.Equal(sut, expected);
        }
    }
}
