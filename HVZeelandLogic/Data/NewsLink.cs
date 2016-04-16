using BaseLogic.Xaml_Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HVZeelandLogic
{
    public sealed class NewsLink : INewsLink
    {
        public string URL { get; private set; }
        public string ImageURL { get; private set; }
        public string Title { get; private set; }
        public string TitleWithTime
        {
            get
            {
                return this.Time + " | " + this.Title;
            }
        }

        public string ContentWithLocation
        {
            get
            {
                return this.Location + " - " + this.Content;
            }
        }

        public string Location { get; private set; }
        public string Content { get; private set; }
        public string CommentCount { get; private set; }
        public string Time { get; private set; }

        public int ImageWidth
        {
            get
            {
                return (this.ImageURL == string.Empty) ? 0 : 140; 
            }
        }

        public int ImageHeight
        {
            get
            {
                return (this.ImageURL == string.Empty) ? 0 : 70;
            }
        }

        public Thickness ContentMargin
        {
            get
            {
                return (this.ImageURL == string.Empty) ? new Thickness(-10, 0, 4.5, 0) : new Thickness(0, 0, 4.5, 0);
            }
        }

        public NewsLink(string URL, string ImageURL, string Location, string Title, string Content, string CommentCount, string Time)
        {
            this.URL = "http://www.hvzeeland.nl/" + URL;
            this.ImageURL = ImageURL.Length > 0 ?  "http://www.hvzeeland.nl/" + ImageURL : string.Empty;
            this.Location = WebUtility.HtmlDecode(Location).Trim();
            this.Title = WebUtility.HtmlDecode(Title);
            this.Content = WebUtility.HtmlDecode(Content).Replace("<br />", "").Replace("\r\n", "").Trim();
            this.CommentCount = WebUtility.HtmlDecode(CommentCount);
            this.Time = WebUtility.HtmlDecode(Time);

            if (this.Location.EndsWith(" -"))
            {
                this.Location = this.Location.Substring(0, this.Location.Length - 2).Trim();
            }

            if (this.Location.EndsWith(" - "))
            {
                this.Location = this.Location.Substring(0, this.Location.Length - 3).Trim();
            }

            if (this.Content.StartsWith(" - "))
            {
                this.Content = this.Content.Substring(3, this.Content.Length - 3).Trim();
            }

            if (this.Content.StartsWith("-"))
            {
                this.Content = this.Content.Substring(2, this.Content.Length - 2).Trim();
            }
        }
    }
}
