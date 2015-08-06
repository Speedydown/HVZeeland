using HVZeeland.Common;
using HVZeelandLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsBackgroundTask;

namespace HVZeeland
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private static IList<NewsLink> newsLinks = null;
        private static IList<P2000Item> P2000Items = null;
        public static DateTime TimeLoaded = DateTime.Now.AddDays(-1);

        private string CurrentURL = string.Empty;
        private bool StopRefresh = false;

        public MainPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public static void ClearCachedData()
        {
            newsLinks = null;
            P2000Items = null;
            TimeLoaded = DateTime.Now.AddDays(-1);
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

            if (newsLinks == null || DateTime.Now.Subtract(TimeLoaded).TotalMinutes > 5)
            {
                await LoadData();
            }

            if (e.NavigationParameter != null && e.NavigationParameter.ToString() != "")
            {
                try
                {
                    await this.OpenNewsItem(e.NavigationParameter.ToString());
                   
                    return;
                }
                catch
                {

                }
            }
        }

        private async Task OpenNewsItem(string URL)
        {
            try
            {
                NewsItemControl.DataContext = null;
                NewsItemLoadingControl.SetLoadingStatus(true);

                if (URL != null)
                {
                    CurrentURL = URL;
                    NewsItem newsItem = await DataHandler.GetNewsPageFromURL(URL);
                    NewsItemControl.DataContext = newsItem;
                }
            }
            catch
            {
                NewsItemLoadingControl.DisplayLoadingError(true);
            }
            finally
            {
                NewsItemLoadingControl.SetLoadingStatus(false);
            }

            await ArticleCounter.AddArticleCount();
            Task t = Task.Run(() => DataHandler.PostAppStats(URL));
        }

        private async Task<IList<NewsLink>> GetNewsLinksOperationAsTask()
        {
            try
            {
                return await DataHandler.GetNewsLinksByPage();
            }
            catch
            {
                LoadingControl.DisplayLoadingError(true);
                ContentGrid.Visibility = Visibility.Collapsed;
                return new List<NewsLink>();
            }
        }

        private async Task<IList<P2000Item>> GetP2000ItemsOperationAsTask()
        {
            try
            {
                return await DataHandler.GetP2000Items();
            }
            catch
            {
                LoadingControl.DisplayLoadingError(true);
                ContentGrid.Visibility = Visibility.Collapsed;
                return new List<P2000Item>();
            }
        }

        private async Task LoadData()
        {
            SearchLoadingControl.SetLoadingStatus(false);
            LoadingControl.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);
            NewsListView.ItemsSource = null;
            P2000ListView.ItemsSource = null;
            ContentGrid.Visibility = Visibility.Visible;
            Task<IList<NewsLink>> GetNewsLinksTask = GetNewsLinksOperationAsTask();
            Task<IList<P2000Item>> GetP2000ItemsTask = GetP2000ItemsOperationAsTask();
            newsLinks = await GetNewsLinksTask;
            NewsListView.ItemsSource = newsLinks;
            P2000Items = await GetP2000ItemsTask;
            P2000ListView.ItemsSource = P2000Items;
            LoadingControl.SetLoadingStatus(false);
            TimeLoaded = DateTime.Now;

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                localSettings.Values["LastNewsItem"] = newsLinks.First().URL;
                NotificationHandler.Run();
            }
            catch
            {

            }

            if (newsLinks.Count > 0)
            {
                await this.OpenNewsItem(newsLinks.First().URL);
            }

            Task RefreshTask = Task.Run(() => this.RefreshData());
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            StopRefresh = true;
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void NewsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await this.OpenNewsItem((e.ClickedItem as NewsLink).URL);
        }

        private async void HVZButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.hvzeeland.nl/"));
        }

        private async void PrivacyPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://wiezitwaarvandaag.nl/privacypolicy.aspx"));
        }

        private async void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadData();

            LoadingControl.SetLoadingStatus(false);
        }

        private void P2000ListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void SearchTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Task Search = this.Search();
            }
        }

        private async Task Search()
        {
            SearchListView.ItemsSource = null;
            SearchLoadingControl.DisplayLoadingError(false);
            SearchLoadingControl.SetLoadingStatus(true);

            try
            {
                SearchListView.ItemsSource = await DataHandler.Search(SearchTextbox.Text);
            }
            catch
            {
                SearchLoadingControl.DisplayLoadingError(true);
            }

            SearchLoadingControl.SetLoadingStatus(false);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Task Search = this.Search();
        }

        private async Task RefreshData()
        {
            while (!StopRefresh)
            {
                await Task.Delay(300000);

                IList<NewsLink> NewsLinks = await GetNewsLinksOperationAsTask();

                if ((NewsLinks.Count > 0 && NewsLinks.First().URL != newsLinks.First().URL) || newsLinks.Count == 0)
                {
                    newsLinks = NewsLinks;
                    NewsListView.ItemsSource = newsLinks;
                }

                IList<P2000Item> p2000Items = await GetP2000ItemsOperationAsTask();

                if ((p2000Items.Count > 0 && p2000Items.First().Content != P2000Items.First().Content) || P2000Items.Count == 0)
                {
                    P2000Items = p2000Items;
                    P2000ListView.ItemsSource = P2000Items;
                }
            }
        }

        #region SwitchButtons
        private void NewsSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            NewsSwitchButton.IsEnabled = false;
            P2000SwitchButton.IsEnabled = true;
            SearchSwitchButton.IsEnabled = true;

            NewsListView.Visibility = Visibility.Visible;
            P2000ListView.Visibility = Visibility.Collapsed;
            SearchGrid.Visibility = Visibility.Collapsed;
        }

        private void P2000SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            NewsSwitchButton.IsEnabled = true;
            P2000SwitchButton.IsEnabled = false;
            SearchSwitchButton.IsEnabled = true;

            NewsListView.Visibility = Visibility.Collapsed;
            P2000ListView.Visibility = Visibility.Visible;
            SearchGrid.Visibility = Visibility.Collapsed;
        }

        private void SearchSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            NewsSwitchButton.IsEnabled = true;
            P2000SwitchButton.IsEnabled = true;
            SearchSwitchButton.IsEnabled = false;

            NewsListView.Visibility = Visibility.Collapsed;
            P2000ListView.Visibility = Visibility.Collapsed;
            SearchGrid.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
