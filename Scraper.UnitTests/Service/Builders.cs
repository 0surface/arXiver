using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

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

            public static string ArticleListRawHtml()
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Scraper.UnitTests.Service.TestData.ArticleListTestHtmlData.html";
                string testData = "";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    testData = reader.ReadToEnd();
                }

                return testData;
            }

            public HtmlDocument ArticleListHtmlDocument()
            {
                doc.LoadHtml(ArticleListRawHtml());
                return doc;
            }

            public static int ArticleListCount => 63;
            public static int FailCaseArticleListCount => 0;

        }
    }
}
