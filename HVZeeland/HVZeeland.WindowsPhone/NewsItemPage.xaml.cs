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
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HVZeeland
{
    public sealed partial class NewsItemPage : Page
    {
        private NavigationHelper navigationHelper;
        private RelayCommand _checkedGoBackCommand;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public NewsItemPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            _checkedGoBackCommand = new RelayCommand(
                                    () => this.CheckGoBack(),
                                    () => this.CanCheckGoBack()
                                );

            navigationHelper.GoBackCommand = _checkedGoBackCommand;
        }

        private bool CanCheckGoBack()
        {
            return true;
        }

        private void CheckGoBack()
        {
            if (NewsItemControl.CanGoBack())
            {
                NavigationHelper.GoBack();
            }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            try
            {
                LoadingControl.SetLoadingStatus(true);

                if (e.NavigationParameter != null)
                {
                    NewsItem newsItem = await DataHandler.GetNewsPageFromURL(e.NavigationParameter.ToString());
                    this.DataContext = newsItem;
                }
            }
            catch
            {
                LoadingControl.DisplayLoadingError(true);
            }
            finally
            {
                LoadingControl.SetLoadingStatus(false);
            }

            await ArticleCounter.AddArticleCount();
            Task t = Task.Run(() => DataHandler.PostAppStats(e.NavigationParameter.ToString()));
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
