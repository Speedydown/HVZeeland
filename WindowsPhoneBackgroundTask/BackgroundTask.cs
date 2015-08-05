using HVZeelandLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace WindowsPhoneBackgroundTask
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            await GenerateNotifications();
            deferral.Complete();
        }

        private async Task GenerateNotifications()
        {
            try
            {
                string LastURL = string.Empty;
                
                if (localSettings.Values["LastNewsItem"] != null)
                {
                    LastURL = localSettings.Values["LastNewsItem"].ToString();
                }
                else
                {
                    return;
                }

                IList<NewsLink> News = await DataHandler.GetNewsLinksByPage();
                IList<NewsLink> NewNewsLinks = new List<NewsLink>();

                int NotificationCounter = 0;

                foreach (NewsLink n in News)
                {
                    if (n.URL == LastURL)
                    {
                        if (NotificationCounter > 0)
                        {
                            CreateTile(NewNewsLinks, NotificationCounter);
                            CreateToast(NewNewsLinks);
                        }

                        return;
                    }

                    NewNewsLinks.Add(n);
                    NotificationCounter++;

                }
            }
            catch (Exception)
            {

            }
        }

        private void CreateToast(IList<NewsLink> Content)
        {
            string LastToast = string.Empty;

            if (localSettings.Values["LastToast"] != null)
            {
                LastToast = localSettings.Values["LastToast"].ToString();
            }

            foreach (NewsLink n in Content)
            {
                if (n.URL == LastToast)
                {
                    break;
                }

                CreateActualToast(n.Title, n.Content, true, n.URL);
            }
        }

        private void CreateActualToast(string TileContent, string SecondaryContent, bool Supress, string ContentURL)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(TileContent));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(SecondaryContent));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.IM");

            toastNode.AppendChild(audio);

            ((XmlElement)toastNode).SetAttribute("launch", ContentURL);
            ToastNotification toast = new ToastNotification(toastXml);
            toast.SuppressPopup = Supress;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void CreateTile(IList<NewsLink> Content, int Counter)
        {
            //LargeTile
            XmlDocument RectangleTile = CreateRectangleTile(Content, Counter);
            XmlDocument SquareTile = CreateSquareTile();
            XmlDocument SmallTile = CreateSmallTile();

            //Badges
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);
            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", Counter.ToString());

            BadgeNotification badge = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

            //Add tiles together
            IXmlNode node = RectangleTile.ImportNode(SquareTile.GetElementsByTagName("binding").Item(0), true);
            RectangleTile.GetElementsByTagName("visual").Item(0).AppendChild(node);

            node = RectangleTile.ImportNode(SmallTile.GetElementsByTagName("binding").Item(0), true);
            RectangleTile.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(RectangleTile);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private XmlDocument CreateRectangleTile(IList<NewsLink> Content, int Counter)
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150IconWithBadgeAndText);
            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");

            try
            {
                tileTextAttributes[0].InnerText = "Laatste nieuws:";
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[1].InnerText = Content[0].Title;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[2].InnerText = Content[1].Title;
            }
            catch
            {

            }

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/Square71x71Logo.scale-240.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "HVZeeland");

            return tileXml;
        }

        private XmlDocument CreateSquareTile()
        {
            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare71x71IconWithBadge);
            XmlNodeList tileImageAttributes = squareTileXml.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/BadgeLogo.scale-100.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "HVZeeland");

            return squareTileXml;
        }

        private XmlDocument CreateSmallTile()
        {
            XmlDocument SmallTIle = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150IconWithBadge);
            XmlNodeList tileImageAttributes = SmallTIle.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/SQUARE71x71Logo.scale-240.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "HVZeeland");

            return SmallTIle;
        }
    }
}
