﻿using System;
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
            return Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"page\">", Input, true));
        }

        private static List<NewsLink> ParseContent(string Input)
        {
            List<string> NewsLinksAsHTML = new List<string>();
            List<NewsLink> NewsLinks = new List<NewsLink>();

            while (Input.Length > 0)
            {
                try
                {
                    NewsLinksAsHTML.Add(HTMLParserUtil.GetContentAndSubstringInput("<div class=\"news-row\">", "<div class=\"clear\"></div>", Input, out Input));
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
                CommentCount = HTMLParserUtil.GetContentAndSubstringInput("Lees verder (", ") </a></div>", Source, out Source, "", false);
            }
            catch
            {
                CommentCount = HTMLParserUtil.GetContentAndSubstringInput("Lees verder/Bekijk video (", ") </a></div>", Source, out Source, "", false);
            }

            return new NewsLink(URL, ImageURL, Location, Title, Content, CommentCount, Time);
        }
    }
}