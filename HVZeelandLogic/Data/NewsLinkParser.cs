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
            //Remove unusable HeaderData
            Source = RemoveHeader(Source);

            //Parse Items
            return ParseContent(Source);
        }

        private static string RemoveHeader(string Input)
        {
            return Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"page\">", Input, true));
        }

        private static List<NewsLink> ParseContent(string Input)
        {
            List<string> NewsLinksAsHTML = new List<string>();
            List<NewsLink> NewsLinks = new List<NewsLink>();

            while (Input.Length > 0)
            {
                int StartIndex = HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"news-row\">", Input, true);

                if (StartIndex == -1)
                {
                    break;
                }

                int EndIndex = HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"clear\"></div>", Input, true);

                if (EndIndex == -1 || EndIndex <= StartIndex)
                {
                    break;
                }

                NewsLinksAsHTML.Add(Input.Substring(StartIndex, EndIndex - StartIndex));

                try
                {
                    Input = Input.Substring(EndIndex);
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

                }
            }

            return NewsLinks;
        }

        private static NewsLink GetNewsLinkFromHTMLSource(string Source)
        {
            #region ImageURL
            int StartIndexOFImageURL = HTMLParserUtil.GetPositionOfStringInHTMLSource("<img src=\"", Source);

            string ImageURL = string.Empty;

            if (StartIndexOFImageURL != -1)
            {
                int EndIndexOfImageURL = HTMLParserUtil.GetPositionOfStringInHTMLSource("\" width=", Source, false);

                if (EndIndexOfImageURL == -1 || StartIndexOFImageURL >= EndIndexOfImageURL)
                {
                    throw new Exception("Invalid URL");
                }

                ImageURL = Source.Substring(StartIndexOFImageURL, EndIndexOfImageURL - StartIndexOFImageURL);
                Source = Source.Substring(EndIndexOfImageURL);
            }
            #endregion

            #region URL
            int StartIndexOFURL = HTMLParserUtil.GetPositionOfStringInHTMLSource("<a href=\"", Source);

            if (StartIndexOFURL == -1)
            {
                throw new Exception("No URL");
            }

            int EndIndexOfURL = HTMLParserUtil.GetPositionOfStringInHTMLSource("\"><span class=\"time\">", Source, false);

            if (EndIndexOfURL == -1 || StartIndexOFURL >= EndIndexOfURL)
            {
                throw new Exception("Invalid URL");
            }
            #endregion
            string URL = Source.Substring(StartIndexOFURL, EndIndexOfURL - StartIndexOFURL);
            Source = Source.Substring(EndIndexOfURL);

            #region Time

            int StartIndexOFTime = HTMLParserUtil.GetPositionOfStringInHTMLSource("class=\"time\">", Source);

            if (StartIndexOFTime == -1)
            {
                throw new Exception("No Time");
            }

            int EndIndexOfTime = HTMLParserUtil.GetPositionOfStringInHTMLSource(" | </span>", Source, false);

            if (EndIndexOfTime == -1 || StartIndexOFTime >= EndIndexOfTime)
            {
                throw new Exception("Invalid Time");
            }

            #endregion
            string Time = Source.Substring(StartIndexOFTime, EndIndexOfTime - StartIndexOFTime);
            Source = Source.Substring(EndIndexOfTime);


            #region Title
            int StartIndexOFTitle = HTMLParserUtil.GetPositionOfStringInHTMLSource("</span>", Source);

            if (StartIndexOFTitle == -1)
            {
                throw new Exception("No Title");
            }

            int EndIndexOFTitle = HTMLParserUtil.GetPositionOfStringInHTMLSource("</a></div>", Source, false);

            if (EndIndexOFTitle == -1 || StartIndexOFTitle >= EndIndexOFTitle)
            {
                throw new Exception("Invalid Title");
            }
            #endregion
            string Title = Source.Substring(StartIndexOFTitle, EndIndexOFTitle - StartIndexOFTitle);
            Source = Source.Substring(EndIndexOFTitle);

            #region Location
            int StartIndexOFLocation = HTMLParserUtil.GetPositionOfStringInHTMLSource("<p><strong>", Source);

            if (StartIndexOFLocation == -1)
            {
                throw new Exception("No Location");
            }

            int EndIndexOFLocation = HTMLParserUtil.GetPositionOfStringInHTMLSource("</strong>", Source, false);

            if (EndIndexOFLocation == -1 || StartIndexOFLocation >= EndIndexOFLocation)
            {
                throw new Exception("Invalid Location");
            }
            #endregion
            string Location = Source.Substring(StartIndexOFLocation, EndIndexOFLocation - StartIndexOFLocation);

            if (Location.EndsWith(" -"))
            {
                Location = Location.Substring(0, Location.Length - 2);
            }

            if (Location.EndsWith(" - "))
            {
                Location = Location.Substring(0, Location.Length - 3);
            }

            Source = Source.Substring(EndIndexOFLocation);

            #region Content
            int StartIndexOFContent = HTMLParserUtil.GetPositionOfStringInHTMLSource("</strong>", Source);

            if (StartIndexOFContent == -1)
            {
                throw new Exception("No content");
            }

            int EndIndexOFContent = HTMLParserUtil.GetPositionOfStringInHTMLSource("</p>", Source, false);

            if (EndIndexOFContent == -1 || StartIndexOFContent >= EndIndexOFContent)
            {
                throw new Exception("Invalid content");
            }
            #endregion
            string Content = Source.Substring(StartIndexOFContent, EndIndexOFContent - StartIndexOFContent);

            if (Content.StartsWith(" - "))
            {
                Content = Content.Substring(3, Content.Length - 3);
            }

            Source = Source.Substring(EndIndexOFContent);

            #region CommentCount
            int StartIndexOFCommentCount = HTMLParserUtil.GetPositionOfStringInHTMLSource("Lees verder (", Source);

            if (StartIndexOFCommentCount == -1)
            {
                StartIndexOFCommentCount = HTMLParserUtil.GetPositionOfStringInHTMLSource("Lees verder/Bekijk video (", Source);
            }

            if (StartIndexOFCommentCount == -1)
            {
                throw new Exception("No CommentCount");
            }

            int EndIndexOFCommentCount = HTMLParserUtil.GetPositionOfStringInHTMLSource(") </a></div>", Source, false);

            if (EndIndexOFCommentCount == -1 || StartIndexOFCommentCount >= EndIndexOFCommentCount)
            {
                throw new Exception("Invalid CommentCount");
            }
            #endregion
            string CommentCount = Source.Substring(StartIndexOFCommentCount, EndIndexOFCommentCount - StartIndexOFCommentCount);
            Source = Source.Substring(EndIndexOFCommentCount);

            return new NewsLink(URL, ImageURL, Location, Title, Content, CommentCount, Time);
        }
    }
}
