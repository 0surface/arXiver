using HtmlAgilityPack;
using Scraper.Service.Scrapers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static Scraper.UnitTests.Service.Builders;

namespace Scraper.UnitTests.Service
{
    public class ArticleListScraperTests : ArticleListTestBuilder
    {
        public ArticleListTestBuilder _articleListTestBuilder = new ArticleListTestBuilder();

        //[Fact]
        [Theory]
        [InlineData("//small", new string[] { "total of", "entries:" }, 63)] //Pass Scenario
        [InlineData("//small", null, 63)]//Pass Scenraio
        [InlineData("", new string[] { "total of", "entries:" }, -1)] //Fail Scenraio
        [InlineData("div [@id='content']", new string[] { "total of", "entries:" }, -1)]//Fail Scenraio        
        public void Get_article_list_Entry_Count_in_htmlDoc(string articleListCountSelector, string[] identifierStrings, int expected)
        {
            //Arrange
            HtmlDocument doc = _articleListTestBuilder.ArticleListHtmlDocument();
            //TODO: Mock repo
            IArticleListScraper scraper = new ArticleListScraper();

            //Act
            var sut = scraper.GetArticleListEntryCount(doc, identifierStrings, articleListCountSelector);

            //Assert
            Assert.Equal(sut, expected);
        }
    }
}
