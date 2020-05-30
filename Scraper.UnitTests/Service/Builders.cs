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
                string no_abstractResourceName = "Scraper.UnitTests.Service.TestData.ArticleList_WithoutAbstract_TestHtmlData.html";
                string with_abstractResourceName = "Scraper.UnitTests.Service.TestData.ArticleList_WithAbstract_TestHtmlData.html";
                return DocReader.ReadDocument(includeAbstract ? with_abstractResourceName : no_abstractResourceName);
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

        public class ArticleContentTestBuilder
        {
            HtmlDocument doc = new HtmlDocument();

            public ArticleContentTestBuilder()
            {

            }

            public HtmlDocument ArticleContent_1_HtmlDocument()
            {
                doc.LoadHtml(DocReader.ReadDocument("Scraper.UnitTests.Service.TestData.ArtcleContent_1_TestData.html"));
                return doc;
            }

            public  int Article_1_AuthorCount => 22;
            public  string Article_1_Comment => "13 pages, 11 figures";
            public  string Article_1_Title => @"[2005.06300] Long-range magnetic order in the ${\tilde S}=1/2$ triangular lattice
  antiferromagnet KCeS$_2$";
        }

        public class DocReader
        {
            public static string ReadDocument(string resourceName)
            {
                string testData = "";
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    testData = reader.ReadToEnd();
                }

                return testData;
            }
        }

    }
}
