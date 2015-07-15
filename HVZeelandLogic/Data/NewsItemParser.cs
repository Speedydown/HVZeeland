using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;

namespace HVZeelandLogic
{
    internal static class NewsItemParser
    {
        public static NewsItem GetNewsItemFromSource(string Source)
        {
            Source = RemoveHeader(Source);
            return GetNewsItemFromHTML(Source);
        }

        private static string RemoveHeader(string Input)
        {
            return Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"news-page\">", Input, true));
        }

        private static NewsItem GetNewsItemFromHTML(string Source)
        {
            string Title = HTMLParserUtil.GetContentAndSubstringInput("<div class=\"title\">", "</div>", Source, out Source, "", false);
            string Updated = HTMLParserUtil.GetContentAndSubstringInput("<div class=\"added-updated\">", "<div class=\"social\">", Source, out Source, "", true);
            string Added = string.Empty;

            try
            {
                string[] TimeStamp = Updated.Split(new string[] { " - " }, StringSplitOptions.None);

                if (TimeStamp.Length == 2)
                {
                    Added =  TimeStamp[0];
                    Updated = TimeStamp[1];
                }
            }
            catch
            {

            }

            string MediaFile = string.Empty;

            try
            {
                MediaFile = HTMLParserUtil.GetContentAndSubstringInput("data-file=\"", "\" data-thumbnail", Source, out Source, "", false);
            }
            catch
            {

            }

            Source = Source.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<div class=\"author\">", Source, false));

            string Author = HTMLParserUtil.GetContentAndSubstringInput("<div class=\"author\">", "</div>", Source, out Source, "", true);
            string ContentSummary = HTMLParserUtil.GetContentAndSubstringInput("<strong>", "</strong>", Source, out Source, "", false);
            string Body = HTMLParserUtil.GetContentAndSubstringInput("</strong>", "</div>", Source, out Source, "", false);

            List<string> Images = null;

            try
            {
                Images = GetImageURLFromHTMLBlock(HTMLParserUtil.GetContentAndSubstringInput("<div class=\"fotos\">", "<div class=\"module-list\">", Source, out Source, "", false));
            }
            catch
            {
                //No Images
                Images = new List<string>();
            }

            List<Comment> Comments = null;

            try
            {
                Comments = GetCommentsFromHTMLBlock(HTMLParserUtil.GetContentAndSubstringInput("<a name=\"reacties\"></a>", "<div class=\"paginator next-prev-numbers\">", Source, out Source, "", false));
            }
            catch
            {
               //No comments
                Comments = new List<Comment>();
            }

            return new NewsItem(Title, Added, Updated, MediaFile, ContentSummary, Body, Images, Author, Comments);
        }

        private static List<string> GetImageURLFromHTMLBlock(string Source)
        {
            List<string> ImageURLs = new List<string>();

            while (Source.Length > 0)
            {
                try
                {
                    string ImageURL = HTMLParserUtil.GetContentAndSubstringInput("src=\"", "\" alt", Source, out Source);
                    ImageURL = ImageURL.Replace("/thumbs", "");
                    ImageURLs.Add(ImageURL);
                }
                catch
                {
                    break;
                }
            }

            return ImageURLs;
        }

        private static List<Comment> GetCommentsFromHTMLBlock(string Source)
        {
            List<Comment> Comments = new List<Comment>();

            while (Source.Length > 0)
            {
                try
                {
                    string Name = HTMLParserUtil.GetContentAndSubstringInput("<b>", "</b>", Source, out Source);
                    string Content = HTMLParserUtil.GetContentAndSubstringInput("<p>", "</p>", Source, out Source);

                    Comments.Add(new Comment(Name, Content));
                }
                catch
                {
                    break;
                }
            }

            return Comments;
        }
    }
}
