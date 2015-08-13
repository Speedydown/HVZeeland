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
using WRCHelperLibrary;

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
                CreateTile(NewNewsLinks.Cast<INewsLink>().ToList(), NewNewsLinks.Count);
                BadgeHandler.CreateBadge(NewNewsLinks.Count);
                ToastHandler.CreateToast(NewNewsLinks);
            }

            deferral.Complete();
        }

        private void CreateTile(IList<INewsLink> Content, int Counter)
        {
            XmlDocument RectangleTile = TileXmlHandler.CreateRectangleTile2(TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText02), Content, Counter, "ms-appx:///assets/Wide310x150Logo.scale-100.png", "HVZeeland");
            XmlDocument SquareTile = TileXmlHandler.CreateLargeSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare310x310Text01), Content);
            XmlDocument SmallTile = TileXmlHandler.CreateSquareTile(TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText03), Content, "ms-appx:///assets/Logo.scale-100.png", "HVZeeland");

            TileXmlHandler.CreateTileUpdate(new XmlDocument[] { RectangleTile, SquareTile, SmallTile });
        }
    }
}
