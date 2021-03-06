﻿using FluentAssertions;
using HtmlAgilityPack;
using Scraper.Service.Scrapers;
using Scraper.Types;
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
            ScrapeResultDto<ArticleItemDto> request = new ScrapeResultDto<ArticleItemDto>();
            int expected = 25;

            //Act
            var sut = _scraper.ScrapeArticleList(request,doc, false);

            //Assert
            sut.Should().NotBeNull();
            sut.Result.Count().Should().Be(expected);
        }

        [Fact]
        public void Gets_article_abstract_text()
        {
            //Arrange
            HtmlDocument doc = _articleListTestBuilder.ArticleList_withAbstract_HtmlDocument();
            ScrapeResultDto<ArticleItemDto> request = new ScrapeResultDto<ArticleItemDto>();
            string expected = _articleListTestBuilder.WithAbstractHtml_ArticleItem2_AbstractText();

            //Act           
            var sut = _scraper.ScrapeArticleList(request, doc, true);

            //Assert
            sut.Result.Should().NotBeEmpty();
            sut.Result.Should().HaveCountGreaterOrEqualTo(2);
            Assert.Equal(sut.Result[1].AbstractText, expected);
        }

        [Fact]
        public void CatchupArchiveArticles_Extracts_AllArticlesIn_HtmlDocument()
        {
            HtmlDocument doc = _articleListTestBuilder.ArticleList_Catchup_SubjectGroup_HtmlDocument();
            ScrapeResultDto<ArticleItemDto> request = new ScrapeResultDto<ArticleItemDto>();
            int expected = 2045;

            var sut = _scraper.ScrapeCatchUpArticleList(request, doc, true);

            sut.Should().NotBeNull();
            sut.Result.Should().HaveCountGreaterOrEqualTo(2);
            sut.Result.Count().Should().Be(expected);
        }

        [Fact]
        public void CatchupArchiveArticles_ExtractsArxivIdLabelText_AllArticlesLabeledAsReplaced()
        {
            HtmlDocument doc = _articleListTestBuilder.ArticleList_Catchup_SubjectGroup_HtmlDocument();
            ScrapeResultDto<ArticleItemDto> request = new ScrapeResultDto<ArticleItemDto>();
            int expected = 1010;

            var sut = _scraper.ScrapeCatchUpArticleList(request, doc, true)
                                .Result
                                .Where(r => r.ArxivIdLabel == "replaced").Count();

            sut.Should().Be(expected);
        }
    }
}
