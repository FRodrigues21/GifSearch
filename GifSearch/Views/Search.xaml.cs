using GifImage;
using GifSearch.Controllers;
using GifSearch.Models;
using GifSearch.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GifSearch.Views
{

    public sealed partial class Search : Page
    {

        private int loaded_count = 0;
        private PlayingItem selected_gif = null;
        private Boolean navigation_caused = true;
        private static Boolean download_started = false;
        private static ResourceLoader res { get; set; }

        public Search()
        {
            this.InitializeComponent();
            NotificationBarFacade.hideStatusBar();
            res = ResourceLoader.GetForCurrentView();
            selected_gif = new PlayingItem();
            navigation_caused = false;
        }

        private void search_GotFocus(object sender, RoutedEventArgs e)
        {
            search.Text = "";
        }

        private void search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            searchQuery(search.Text);
        }

        private void search_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                loseFocus(sender);
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();
            }
        }

        private void loseFocus(object sender)
        {
            var control = sender as Control;
            var isTabStop = control.IsTabStop;
            control.IsTabStop = false;
            control.IsEnabled = false;
            control.IsEnabled = true;
            control.IsTabStop = isTabStop;
        }

        private void gif_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                Tabs.playGifAnimation((ListView)sender, ((ListView)sender).SelectedItem);
            else
                Tabs.playGifAnimation((GridView)sender, ((GridView)sender).SelectedItem);
        }

        private void gif_list_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)((GridView)sender).ItemsPanelRoot;
            panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / 4;
        }

        private async void gif_image_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded_count >= 15)
            {
                await Task.Delay(3000);
                NotificationBarFacade.hideStatusBar();
                loaded_count = 0;
            }
            else
            {
                loaded_count++;
            }
        }

        private async void searchQuery(String text)
        {
            if (search.Text != null && App.pivot_index == 1)
            {
                NotificationBarFacade.displayStatusBarMessage(res.GetString("SearchMessage_Loading"), false);
                App.search = await GifRiffsyFacade.searchGif(text);
                if (App.search == null)
                {
                    error_presenter.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                        gif_list.ItemsSource = new TrendingToShow(ProgressBar, App.search, App.search.Count, 5);
                    else
                        gif_list.ItemsSource = App.search;
                    error_presenter.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void refresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            searchQuery(search.Text);
        }

        private void gif_image_ImageOpened(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Opened");
        }

    }
}
