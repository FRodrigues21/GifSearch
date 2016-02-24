using GifSearch.Models;
using Microsoft.Advertising.WinRT.UI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.IO.IsolatedStorage;
using System;
using Windows.UI.Popups;
using Windows.ApplicationModel.Store;
using GifSearch.Exceptions;
using Windows.System;

namespace GifSearch
{

    public sealed partial class MainPage : Page
    {

        private string current_text = "";
        private Pivot pivot = null;

        public MainPage()
        {
            this.InitializeComponent();
            image_riffsy.Visibility = Visibility.Collapsed;
            reviewfunction();
            pivot = pivot_app;
            list_gifs_trending_load();
        }

        public async void reviewfunction()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string Appname = "gif Search";

            if (!settings.Values.ContainsKey("review"))
            {
                settings.Values.Add("review", 1);
                settings.Values.Add("rcheck", 0);
            }
            else
            {
                int no = Convert.ToInt32(settings.Values["review"]);
                int check = Convert.ToInt32(settings.Values["rcheck"]);
                no++;
                if ((no % 5 == 0) && check == 0)
                {
                    settings.Values["review"] = no;
                    MessageDialog mydial = new MessageDialog("Thank you for using this application.\nWould you like to give some time to rate and review this application to help us improve");
                    mydial.Title = Appname;
                    mydial.Commands.Add(new UICommand(
                        "Yes",
                        new UICommandInvokedHandler(this.CommandInvokedHandler_yesclick)));
                    mydial.Commands.Add(new UICommand(
                       "No",
                       new UICommandInvokedHandler(this.CommandInvokedHandler_noclick)));
                    await mydial.ShowAsync();

                }
                else
                {
                    settings.Values["review"] = no;
                }
            }
        }

        private void CommandInvokedHandler_noclick(IUICommand command)
        {

        }

        private async void CommandInvokedHandler_yesclick(IUICommand command)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["rcheck"] = 1;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
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
            refresh_trending.Visibility = Visibility.Collapsed;
            list_gifs_trending.Visibility = Visibility.Collapsed;
            progressring_loading_trending.IsActive = true;
            if (App.source.Equals("riffsy"))
            {
                var collection = (ObservableCollection<Result>)null;
                try
                {
                    collection = await GifRiffsyFacade.getTrending();
                }
                catch (Exception e)
                {
                    MessageDialog mydial = new MessageDialog("It seems that you have no Internet connection\nFix the problem and try again!");
                    mydial.Title = "gif Search";
                    mydial.Commands.Add(new UICommand(
                        "Try Again",
                        new UICommandInvokedHandler(this.CommandInvokedHandler_tryagain_trending)));
                    await mydial.ShowAsync();
                    refresh_trending.Visibility = Visibility.Visible;
                }

                list_gifs_trending.ItemsSource = collection;
            }
            else if (App.source.Equals("giphy"))
            {
                var collection = (ObservableCollection<Datum>)null;
                try
                {
                    collection = await GifGiphyFacade.getTrending();
                } catch(Exception e)
                {
                    MessageDialog mydial = new MessageDialog("It seems that you have no Internet connection\nFix the problem and try again!");
                    mydial.Title = "gif Search";
                    mydial.Commands.Add(new UICommand(
                        "Try Again",
                        new UICommandInvokedHandler(this.CommandInvokedHandler_tryagain_trending)));
                    await mydial.ShowAsync();
                    refresh_trending.Visibility = Visibility.Visible;
                }
                
                list_gifs_trending.ItemsSource = collection;
            }
            progressring_loading_trending.IsActive = false;
            list_gifs_trending.Visibility = Visibility.Visible;
        }

        private void CommandInvokedHandler_tryagain_trending(IUICommand command)
        {
            
        }

        private void CommandInvokedHandler_tryagain_search(IUICommand command)
        {

        }

        private void button_search_Click(object sender, RoutedEventArgs e)
        {
            if (textbox_search.Text != null && !textbox_search.Text.Equals(current_text))
            {
                searchClick();
            }
        }

        private async void searchClick()
        {
            if (textbox_search.Text != null)
            {
                refresh_search.Visibility = Visibility.Visible;
                list_gifs_search.Visibility = Visibility.Collapsed;
                progressring_loading.IsActive = true;
                if (App.source.Equals("riffsy"))
                {
                    var collection = (ObservableCollection<Result>)null;
                    try
                    {
                        collection = await GifRiffsyFacade.searchGif(textbox_search.Text);
                    }
                    catch (Exception e)
                    {
                        MessageDialog mydial = new MessageDialog("It seems that you have no Internet connection\nFix the problem and try again!");
                        mydial.Title = "gif Search";
                        mydial.Commands.Add(new UICommand(
                            "Try Again",
                            new UICommandInvokedHandler(this.CommandInvokedHandler_tryagain_search)));
                        await mydial.ShowAsync();
                        refresh_search.Visibility = Visibility.Visible;
                    }

                    list_gifs_search.ItemsSource = collection;
                }

                else if (App.source.Equals("giphy"))
                {
                    var collection = (ObservableCollection<Datum>)null;
                    try
                    {
                        collection = await GifGiphyFacade.searchGif(textbox_search.Text);
                    }
                    catch (Exception e)
                    {
                        MessageDialog mydial = new MessageDialog("It seems that you have no Internet connection\nFix the problem and try again!");
                        mydial.Title = "gif Search";
                        mydial.Commands.Add(new UICommand(
                            "Try Again",
                            new UICommandInvokedHandler(this.CommandInvokedHandler_tryagain_search)));
                        await mydial.ShowAsync();
                        refresh_search.Visibility = Visibility.Visible;
                    }

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
            if (listitem_giphy.IsSelected)
            {
                App.source = "giphy";
                image_giphy.Visibility = Visibility.Visible;
                image_riffsy.Visibility = Visibility.Collapsed;
            }
            else if (listitem_riffsy.IsSelected)
            {
                App.source = "riffsy";
                image_giphy.Visibility = Visibility.Collapsed;
                image_riffsy.Visibility = Visibility.Collapsed;
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

        private void refresh_trending_Click(object sender, RoutedEventArgs e)
        {
            list_gifs_trending_load();
        }

        private void refresh_search_Click(object sender, RoutedEventArgs e)
        {
            searchClick();
        }

        private async void reddit_link_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/WPDev/comments/46afdi/hey_developers_can_you_make_a_simple_gif_search/"));
        }
    }
}
