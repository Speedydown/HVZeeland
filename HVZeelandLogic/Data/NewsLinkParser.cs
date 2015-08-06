using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace HVZeelandLogic
{
    internal static class NewsLinkParser
    {
        public static IList<NewsLink> GetNewsLinksFromSource(string Source)
        {
            Source = RemoveHeader(Source);
            return ParseContent(Source);
        }

        private static string RemoveHeader(string Input)
        {
            return Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"news-", Input, false));
        }

        private static List<NewsLink> ParseContent(string Input)
        {
            List<string> NewsLinksAsHTML = new List<string>();
            List<NewsLink> NewsLinks = new List<NewsLink>();

            while (Input.Length > 0)
            {
                try
                {
                    NewsLinksAsHTML.Add(HTMLParserUtil.GetContentAndSubstringInput("<div class=\"news-", ") </a></div>", Input, out Input));
                }
                catch
                {
                    break;
                }
            }

            if (NewsLinksAsHTML.Count == 0)
            {
                throw new Exception("No Items");
            }

            foreach (string s in NewsLinksAsHTML)
            {
                try
                {
                    NewsLinks.Add(GetNewsLinkFromHTMLSource(s));
                }
                catch (Exception)
                {
                    //Exception?
                }
            }

            return NewsLinks;
        }

        private static NewsLink GetNewsLinkFromHTMLSource(string Source)
        {
            if (Source.Contains("column-left") || Source.Contains("column-right"))
            {
                return GetNewsLinkFromHTMLSourceInColumns(Source);
            }

            string OriginalSource = Source;
            string ImageURL = string.Empty;

            try
            {
                ImageURL = HTMLParserUtil.GetContentAndSubstringInput("<img src=\"", "\" width=", Source, out Source);
            }
            catch
            {

            }

            string URL = HTMLParserUtil.GetContentAndSubstringInput("<a href=\"", "\"><span class=\"time\">", Source, out Source, "", false);
            string Time = HTMLParserUtil.GetContentAndSubstringInput("class=\"time\">", " | </span>", Source, out Source, "", false);
            string Title = HTMLParserUtil.GetContentAndSubstringInput("</span>", "</a></div>", Source, out Source, "", false);
            string Location = HTMLParserUtil.GetContentAndSubstringInput("<p><strong>", "</strong>", Source, out Source, "", false);
            string Content = HTMLParserUtil.GetContentAndSubstringInput("</strong>", "</p>", Source, out Source, "", false);

            string CommentCount = string.Empty;

            try
            {
                CommentCount = HTMLParserUtil.GetContentAndSubstringInput("Lees verder (", " reacties", Source, out Source, "", false) + " reacties";
            }
            catch
            {
                CommentCount = HTMLParserUtil.GetContentAndSubstringInput("Lees verder/Bekijk video (", " reacties", Source, out Source, "", false) + " reacties";
            }

            return new NewsLink(URL, ImageURL, Location, Title, Content, CommentCount, Time);
        }

        private static NewsLink GetNewsLinkFromHTMLSourceInColumns(string Source)
        {
            string URL = HTMLParserUtil.GetContentAndSubstringInput("<a href=\"", "\"><span class=\"time\">", Source, out Source, "", false);
            string Time = HTMLParserUtil.GetContentAndSubstringInput("class=\"time\">", " | </span>", Source, out Source, "", false);
            string Title = HTMLParserUtil.GetContentAndSubstringInput("</span>", "</a></div>", Source, out Source, "", false);


            string ImageURL = string.Empty;

            try
            {
                ImageURL = HTMLParserUtil.GetContentAndSubstringInput("<img src=\"", "\" width=", Source, out Source);
            }
            catch
            {

            }

            

            string Location = HTMLParserUtil.GetContentAndSubstringInput("<p><strong>", "</strong>", Source, out Source, "", false);
            string Content = HTMLParserUtil.GetContentAndSubstringInput("</strong>", "</p>", Source, out Source, "", false);

            string CommentCount = string.Empty;

            try
            {
                CommentCount = HTMLParserUtil.GetContentAndSubstringInput("Lees verder (", " reactie", Source, out Source, "", false) + " reactie(s)";
            }
            catch
            {
                CommentCount = HTMLParserUtil.GetContentAndSubstringInput("Lees verder/Bekijk video (", " reactie", Source, out Source, "", false) + " reactie(s)";
            }

            return new NewsLink(URL, ImageURL, Location, Title, Content, CommentCount, Time);
        }
    }
}
