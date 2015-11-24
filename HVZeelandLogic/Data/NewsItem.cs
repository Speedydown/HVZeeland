using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WRCHelperLibrary;

namespace HVZeelandLogic
{
    public sealed class NewsItem : INewsItem
    {
        public string Title { get; private set; }
        public string Added { get; private set; }
        public string Updated { get; private set; }

        public string TimeStamp
        {
            get
            {
                if (Added == string.Empty)
                {
                    return Updated;
                }
                else
                {
                    return Added + "\n" + Updated;
                }
            }
        }

        public Brush TitleColor
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        public Brush TitleColorWindows
        {
            get { return new SolidColorBrush(Color.FromArgb((byte)255, (byte)9, (byte)104, (byte)152)); }
        }

        public Visibility MediaVisibilty
        {
            get
            {
                return MediaFile == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility TimeStampVisibilty
        {
            get
            {
                return Visibility.Visible;
            }
        }

        public Visibility SummaryVisibilty
        {
            get 
            {
                return Visibility.Visible;
            }
        }

        public Thickness ContentMargins
        {
            get
            {
                return new Thickness(4, 15, 4, 5);
            }
        }

        public Uri MediaFile { get; private set; }
        public string ContentSummary { get; private set; }
        public IList<string> Body { get; private set; }
        public IList<string> ImageList { get; private set; }
        public string Author { get; private set; }
        public IList<Comment> Comments { get; private set; }

        public Uri YoutubeURL { get; private set; }
        public Visibility DisplayWebView
        {
            get
            {
                return YoutubeURL == null ? Visibility.Collapsed : Visibility.Visible;
            }

        }

        public NewsItem(string Title, string Added, string Updated, string MediaFile, string ContentSummary, string Body, IList<string> ImageList, string Author, IList<Comment> Comments)
        {
            this.Title = HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(Title)).Trim();
            this.Added = HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(Added)).Trim();
            this.Updated = HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(Updated)).Trim();

            if (MediaFile.Length != 0)
            {
                MediaFile = !MediaFile.StartsWith("http") ? "http://www.hvzeeland.nl" + MediaFile : MediaFile;
                this.MediaFile = new Uri(HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(MediaFile)).Trim());
            }
            this.Author = HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(Author)).Trim();
            this.ContentSummary = HTMLParserUtil.CleanHTMLTagsFromString(HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(ContentSummary))).Trim();
            this.Body = new string[] { HTMLParserUtil.CleanHTMLTagsFromString(HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(Body)).Trim())}.ToList();
            this.ImageList = ImageList;
            this.Comments = Comments;
        }
    }

    public sealed class Comment
    {
        public string Name { get; private set; }
        public string Content { get; private set; }

        public Comment(string Name, string Content)
        {
            this.Name = HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(Name)).Trim();
            this.Content = HTMLParserUtil.CleanHTMLString(WebUtility.HtmlDecode(Content)).Trim();
        }
    }
}
