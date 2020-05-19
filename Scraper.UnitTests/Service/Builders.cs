using HtmlAgilityPack;
using System.IO;
using System.Reflection;

namespace Scraper.UnitTests.Service
{
    public class Builders
    {
        public class ArticleListTestBuilder
        {
            HtmlDocument doc = new HtmlDocument();
            public int CorrectArticleListCount { get; set; }
            public int InvalidArticleListCount { get; set; }

            public ArticleListTestBuilder()
            {
                CorrectArticleListCount = 63;
                InvalidArticleListCount = -1;
            }

            public static string ArticleListRawHtml(bool includeAbstract)
            {
                var assembly = Assembly.GetExecutingAssembly();
                string no_abstractResourceName = "Scraper.UnitTests.Service.TestData.ArticleList_WithoutAbstract_TestHtmlData.html";
                string with_abstractResourceName = "Scraper.UnitTests.Service.TestData.ArticleList_WithAbstract_TestHtmlData.html";
                string testData = "";

                using (Stream stream = assembly.GetManifestResourceStream(includeAbstract ? with_abstractResourceName : no_abstractResourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    testData = reader.ReadToEnd();
                }

                return testData;
            }

            public HtmlDocument ArticleList_withAbstract_HtmlDocument()
            {
                doc.LoadHtml(ArticleListRawHtml(true));
                return doc;
            }

            public HtmlDocument ArticleList_withoutAbstract_HtmlDocument()
            {
                doc.LoadHtml(ArticleListRawHtml(false));
                return doc;
            }

            public static int ArticleListCount => 63;
            public static int FailCaseArticleListCount => 0;
            public string WithAbstractHtml_ArticleItem2_AbstractText()
            {
                return @"Bitcoin represents one of the most interesting technological breakthroughs
                            and socio-economic experiments of the last decades. In this paper, we examine
                            the role of speculative bubbles in the process of Bitcoin's technological
                            adoption by analyzing its social dynamics. We trace Bitcoin's genesis and
                            dissect the nature of its techno-economic innovation. In particular, we present
                            an analysis of the techno-economic feedback loops that drive Bitcoin's price
                            and network effects. Based on our analysis of Bitcoin, we test and further
                            refine the Social Bubble Hypothesis, which holds that bubbles constitute an
                            essential component in the process of technological innovation. We argue that a
                            hierarchy of repeating and exponentially increasing series of bubbles and hype
                            cycles, which has occurred over the past decade since its inception, has
                            bootstrapped Bitcoin into existence.";
            }


        }
    }
}
