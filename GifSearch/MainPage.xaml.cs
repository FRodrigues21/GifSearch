using GifSearch.Models;
using Microsoft.Advertising.WinRT.UI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace GifSearch
{

    public sealed partial class MainPage : Page
    {

        private string current_text = "";
        private Pivot pivot = null;

        public MainPage()
        {
            this.InitializeComponent();
            pivot = pivot_app;
            list_gifs_trending_load();
        }

        private void ad_microsoft_AdRefreshed(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Ad refreshed.");
        }

        private void ad_microsoft_ErrorOccurred(object sender, AdErrorEventArgs e)
        {
            Debug.WriteLine("Ad loading error.");
        }

        private async void list_gifs_trending_load()
        {
            list_gifs_trending.Visibility = Visibility.Collapsed;
            progressring_loading_trending.IsActive = true;
            if (App.source.Equals("riffsy"))
            {
                var collection = await GifRiffsyFacade.getTrending();
                list_gifs_trending.ItemsSource = collection;
            }
            else if(App.source.Equals("giphy"))
            {
                var collection = await GifGiphyFacade.getTrending();
                list_gifs_trending.ItemsSource = collection;
            }
            progressring_loading_trending.IsActive = false;
            list_gifs_trending.Visibility = Visibility.Visible;
        }

        private void button_search_Click(object sender, RoutedEventArgs e)
        {
            if(textbox_search.Text != null && !textbox_search.Text.Equals(current_text))
            {
                searchClick();
            }
        }

        private async void searchClick()
        {
            if (textbox_search.Text != null)
            {
                list_gifs_search.Visibility = Visibility.Collapsed;
                progressring_loading.IsActive = true;
                if (App.source.Equals("riffsy"))
                {
                    var collection = await GifRiffsyFacade.searchGif(textbox_search.Text);
                    list_gifs_search.ItemsSource = collection;
                }
                
                else if (App.source.Equals("giphy"))
                {
                    var collection = await GifGiphyFacade.searchGif(textbox_search.Text);
                    list_gifs_search.ItemsSource = collection;
                }
                progressring_loading.IsActive = false;
                list_gifs_search.Visibility = Visibility.Visible;
                current_text = textbox_search.Text;
            }
        }

        private void list_gifs_search_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)list_gifs_search.ItemsPanelRoot;
            panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / 2;
        }

        private async void showNotification()
        {
            row_notification.Height = new GridLength(20, GridUnitType.Pixel);
            await Task.Delay(3000);
            row_notification.Height = new GridLength(0, GridUnitType.Pixel);
        }

        private void list_gifs_trending_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)list_gifs_trending.ItemsPanelRoot;
            panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / 2;
        }

        private void button_filter_Click(object sender, RoutedEventArgs e)
        {
            grid_popup_StateChange();
        }

        private void listbox_filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grid_popup_StateChange();
            if(listitem_giphy.IsSelected)
            {
                App.source = "giphy";
                image_giphy.Visibility = Visibility.Visible;
                image_riffsy.Visibility = Visibility.Collapsed;
            }
            else if(listitem_riffsy.IsSelected)
            {
                App.source = "riffsy";
                image_giphy.Visibility = Visibility.Collapsed;
                image_riffsy.Visibility = Visibility.Visible;
            }
            App.changed = true;
            if (pivot.SelectedIndex == 0)
            {
                list_gifs_trending_load();
            }
            else if (pivot.SelectedIndex == 1)
            {
                searchClick();
            }
        }

        private void grid_popup_StateChange()
        {

            if (grid_popup.Visibility == Visibility.Collapsed)
                grid_popup.Visibility = Visibility.Visible;
            else
                grid_popup.Visibility = Visibility.Collapsed;
        }

        private void pivot_app_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    if (App.changed)
                        list_gifs_trending_load();
                    App.changed = false;
                    break;

                case 1:
                    if (App.changed)
                        searchClick();
                    App.changed = false;
                    break;
            }
        }

        private void textbox_search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (textbox_search.Text != null && !textbox_search.Text.Equals(current_text))
            {
                searchClick();
            }
        }

        private void textbox_search_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();
            }
        }

        private void list_gifs_search_ItemClick(object sender, ItemClickEventArgs e)
        {
            showNotification();
            var dataPackage = new DataPackage();
            string text = "";
            if (App.source.Equals("riffsy"))
            {
                Result result = e.ClickedItem as Result;
                text = result.image_link;
            }
            else if (App.source.Equals("giphy"))
            {
                Datum datum = e.ClickedItem as Datum;
                text = datum.image_link;
            }
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        private void list_gifs_trending_ItemClick(object sender, ItemClickEventArgs e)
        {
            showNotification();
            var dataPackage = new DataPackage();
            string text = "";
            if (App.source.Equals("riffsy"))
            {
                Result result = e.ClickedItem as Result;
                text = result.image_link;
            }
            else if (App.source.Equals("giphy"))
            {
                Datum datum = e.ClickedItem as Datum;
                text = datum.image_link;
            }
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
