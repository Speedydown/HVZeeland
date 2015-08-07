using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace HVZeelandLogic
{
    public static class NotificationDataHandler
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static IAsyncOperation<IList<NewsLink>> GenerateNotifications()
        {
            return GenerateNotificationsHelper().AsAsyncOperation();
        }

        private static async Task<IList<NewsLink>> GenerateNotificationsHelper()
        {
            IList<NewsLink> NewNewsLinks = new List<NewsLink>();

            try
            {
                string LastURL = string.Empty;

                if (localSettings.Values["LastNewsItem"] != null)
                {
                    LastURL = localSettings.Values["LastNewsItem"].ToString();
                }
                else
                {
                    return NewNewsLinks;
                }

                IList<NewsLink> News = await DataHandler.GetNewsLinksByPage();
                int NotificationCounter = 0;

                //Test Code
                //LastURL = News[3].URL;

                foreach (NewsLink n in News)
                {
                    if (n.URL == LastURL)
                    {
                        if (NotificationCounter > 0)
                        {
                            return NewNewsLinks;
                        }
                    }

                    NewNewsLinks.Add(n);
                    NotificationCounter++;
                }
            }
            catch (Exception)
            {

            }

            return new List<NewsLink>();
        }
    }
}
