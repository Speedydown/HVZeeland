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
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsPhoneBackgroundTask;
using WRCHelperLibrary;

namespace HVZeeland
{
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private static IList<NewsLink> newsLinks = null;
        private static IList<P2000Item> P2000Items = null;
        public static DateTime TimeLoaded = DateTime.Now.AddDays(-1);

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
            StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
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
                    Frame.Navigate(typeof(NewsItemPage), (e.NavigationParameter));
                    return;
                }
                catch
                {

                }
            }
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
                LoadingControlP2000.DisplayLoadingError(true);
                return new List<P2000Item>();
            }
        }

        private async Task LoadData()
        {
            LoadingControl.DisplayLoadingError(false);
            LoadingControlP2000.DisplayLoadingError(false);
            LoadingControl.SetLoadingStatus(true);
            LoadingControlP2000.SetLoadingStatus(true);
            SearchLoadingControl.SetLoadingStatus(false);
            NewsListView.ItemsSource = null;
            P2000ListView.ItemsSource = null;
            Task<IList<NewsLink>> GetNewsLinksTask = GetNewsLinksOperationAsTask();
            Task<IList<P2000Item>> GetP2000ItemsTask = GetP2000ItemsOperationAsTask();

            newsLinks = await GetNewsLinksTask;
            LoadingControl.SetLoadingStatus(false);
            NewsListView.ItemsSource = newsLinks;

            P2000Items = await GetP2000ItemsTask;
            LoadingControlP2000.SetLoadingStatus(false);
            P2000ListView.ItemsSource = P2000Items;

            TimeLoaded = DateTime.Now;

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            try
            {
                localSettings.Values["LastNewsItem"] = newsLinks.First().URL;
                NotificationHandler.Run("WindowsPhoneBackgroundTask.BackgroundTask", "HVZeelandBackgroundWorker");
            }
            catch
            {

            }
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

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

        private void NewsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(NewsItemPage), (e.ClickedItem as NewsLink).URL);
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

                var control = sender as Control;
                var isTabStop = control.IsTabStop;
                control.IsTabStop = false;
                control.IsEnabled = false;
                control.IsEnabled = true;
                control.IsTabStop = isTabStop;
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

        private void HVZPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HVZPivot.SelectedItem == Archive)
            {
                ReloadButton.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Visible;

            }
            else
            {
                ReloadButton.Visibility = Visibility.Visible;
                SearchButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Task Search = this.Search();

            var control = sender as Control;
            var isTabStop = control.IsTabStop;
            control.IsTabStop = false;
            control.IsEnabled = false;
            control.IsEnabled = true;
            control.IsTabStop = isTabStop;
        }
    }
}
