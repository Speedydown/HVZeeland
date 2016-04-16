using BaseLogic.HtmlUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            if (HTTPGetUtil.Cookiejar == null || HTTPGetUtil.Cookiejar.Count == 0)
            {
                await HTTPGetUtil.GetDataAsStringFromURL("http://www.hvzeeland.nl/accepteer-cookies?next=http://www.hvzeeland.nl/");
            }   

            string PageSource = await HTTPGetUtil.GetDataAsStringFromURL("http://www.hvzeeland.nl/");

            return NewsLinkParser.GetNewsLinksFromSource(PageSource);
        }

        public static IAsyncOperation<NewsItem> GetNewsPageFromURL(string URL)
        {
            return GetNewsPageFromURLHelper(URL).AsAsyncOperation();
        }

        private static async Task<NewsItem> GetNewsPageFromURLHelper(string URL)
        {
            string PageSource = await HTTPGetUtil.GetDataAsStringFromURL(URL);

            return NewsItemParser.GetNewsItemFromSource(PageSource);
        }

        public static IAsyncOperation<IList<P2000Item>> GetP2000Items()
        {
            return GetP2000ItemsHelper().AsAsyncOperation();
        }

        private static async Task<IList<P2000Item>> GetP2000ItemsHelper()
        {
            string PageSource = await HTTPGetUtil.GetDataAsStringFromURL("http://www.p2000zeeland.nl/hvz/hvzeeland.php");

            return P2000Parser.GetP2000ItemsFromSource(PageSource);
        }

        public static IAsyncOperation<IList<NewsLink>> Search(string SearchTerm)
        {
             return SearchHelper(SearchTerm).AsAsyncOperation();
        }

        private static async Task<IList<NewsLink>> SearchHelper(string SearchTerm)
        {
            string PageSource = await HTTPGetUtil.GetDataAsStringFromURL("http://www.hvzeeland.nl/zoeken/resultaten?zoekwoord=" + SearchTerm + "&van=&tot=");

            return NewsLinkParser.GetNewsLinksFromSource(PageSource);
        }
    }
}
