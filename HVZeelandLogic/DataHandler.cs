using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerTools;
using Windows.Foundation;

namespace HVZeelandLogic
{
    public static class DataHandler
    {
        public static IAsyncOperation<IList<NewsLink>> GetNewsLinksByPage()
        {
            return GetNewsLinksByPageHelper().AsAsyncOperation();
        }

        private static async Task<IList<NewsLink>> GetNewsLinksByPageHelper()
        {
            string PageSource = await HTTPGetUtil.GetDataAsStringFromURL("http://www.hvzeeland.nl/");

            return NewsLinkParser.GetNewsLinksFromSource(PageSource);
        }
    }
}
