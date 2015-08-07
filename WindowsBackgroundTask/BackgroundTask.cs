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

namespace WindowsBackgroundTask
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            IList<NewsLink> NewNewsLinks = await NotificationDataHandler.GenerateNotifications();

            if (NewNewsLinks.Count > 0)
            {
                CreateTile(NewNewsLinks, NewNewsLinks.Count);
                ToastHandler.CreateToast(NewNewsLinks);
            }

            deferral.Complete();
        }

        private void CreateTile(IList<NewsLink> Content, int Counter)
        {
            //LargeTile
            XmlDocument RectangleTile = CreateRectangleTile(Content, Counter);
            XmlDocument SquareTile = CreateSquareTile(Content);
            XmlDocument SmallTile = CreateSmallTile(Content);

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
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText02);
            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");


            try
            {
                tileTextAttributes[0].InnerText = Content[0].Title;
            }
            catch
            {

            }

            try
            {
                tileTextAttributes[1].InnerText = Content[1].Title;
            }
            catch
            {

            }

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/Wide310x150Logo.scale-100.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "HVZeeland");

            return tileXml;
        }

        private XmlDocument CreateSquareTile(IList<NewsLink> Content)
        {
            int ContentCounter = 9;

            if (Content.Count < 9)
            {
                ContentCounter = Content.Count;
            }

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310Text01);

            XmlNodeList tileTextAttributes = squareTileXml.GetElementsByTagName("text");


            try
            {
                tileTextAttributes[0].InnerText = "Laatste nieuws:";
            }
            catch
            {

            }

            for (int i = 0; i < ContentCounter; i++)
            {
                try
                {
                    tileTextAttributes[i + 1].InnerText = Content[i].Content;
                }
                catch
                {

                }
            }

               

            return squareTileXml;
        }

        private XmlDocument CreateSmallTile(IList<NewsLink> Content)
        {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText03);
            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");

            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/Logo.scale-100.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "HVZeeland");

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

            try
            {
                tileTextAttributes[3].InnerText = Content[2].Title;
            }
            catch
            {

            }

            return tileXml;
        }
    }
}
