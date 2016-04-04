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
using Windows.UI;
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

    public sealed partial class Trending : Page
    {

        private int loaded_count = 0;
        private PlayingItem selected_gif = null;
        private Boolean navigation_caused = true;
        private static Boolean download_started = false;
        private static ResourceLoader res { get; set; }

        public Trending()
        {
            this.InitializeComponent();
            NotificationBarFacade.hideStatusBar();
            res = ResourceLoader.GetForCurrentView();
            this.loadGifList();
            selected_gif = new PlayingItem();
            navigation_caused = false;
        }

        private void gif_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                Tabs.playGifAnimation((ListView)sender, ((ListView)sender).SelectedItem);
            else
                Tabs.playGifAnimation((GridView)sender, ((GridView)sender).SelectedItem);
        }

        private async void loadGifList()
        {
            if(App.pivot_index == 0)
            {
                NotificationBarFacade.displayStatusBarMessage(res.GetString("TrendingMessage_Loading"), false);
                App.trending = await GifRiffsyFacade.getTrending();
                if (App.trending == null)
                {
                    error_presenter.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                        gif_list.ItemsSource = new TrendingToShow(ProgressBar, App.trending, App.trending.Count, 5);
                    else
                        gif_list.ItemsSource = App.trending;
                    error_presenter.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void gif_list_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)((GridView)sender).ItemsPanelRoot;
            panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / 4;
        }

        private async void gif_image_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded_count >= 20)
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

        private void refresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            loadGifList();
        }

    }
}
