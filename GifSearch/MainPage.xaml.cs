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
using Windows.System;
using GifImage;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Animation;

namespace GifSearch
{

    public sealed partial class MainPage : Page
    {

        private string current_text = "";
        private Pivot pivot = null;
        private GifImageSource _item_playing = null;

        public MainPage()
        {
            this.InitializeComponent();
            image_riffsy.Visibility = Visibility.Collapsed;
            reviewfunction();
            pivot = pivot_app;
            changeLogShow();
        }

        public async void changeLogShow()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if(!settings.Values.ContainsKey("use"))
            {
                settings.Values.Add("use", 0);
                MessageDialog mydial = new MessageDialog("1.2.0.1\n\n-Gifs don't auto-play at start\n- Click gif to start playing\n- Long press gif to copy link to keyboard\n- Gifs play only 3 times (prevent HIGH CPU usage)\n\nMore features will be added in the future!");
                mydial.Title = "What's new in gif Search?";
                mydial.Commands.Add(new UICommand(
                    "Continue to app",
                    new UICommandInvokedHandler(this.CommandInvokedHandler_continueclick)));
                mydial.Commands.Add(new UICommand(
                   "Rate the app",
                   new UICommandInvokedHandler(this.CommandInvokedHandler_yesclick)));
                await mydial.ShowAsync();
            }
            else
            {
                list_gifs_trending_load();
            }
        }

        private void CommandInvokedHandler_continueclick(IUICommand command)
        {
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
            list_gifs_trending_load();
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["rcheck"] = 1;
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp"));
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
                refresh_search.Visibility = Visibility.Collapsed;
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
            row_notification.Height = 20;
            await Task.Delay(3000);
            row_notification.Height = 0;
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

        private void list_gifs_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_item_playing != null)
                _item_playing.Pause();
            list_gifs_trending.UpdateLayout();
            var _container = list_gifs_search.ContainerFromItem(list_gifs_search.SelectedItem);
            var _children = allChildren(_container);
            var _control = _children.OfType<Image>().First(x => x.Name == "gif_image");

            GifImageSource _gif = AnimationBehavior.GetGifImageSource(_control);
            if (_gif != null)
            {
                AnimationBehavior.SetRepeatBehavior(_control, new RepeatBehavior(3));
                _gif.Start();
            }

            _item_playing = _gif;
        }

        private void list_gifs_trending_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_item_playing != null)
                _item_playing.Pause();
            list_gifs_trending.UpdateLayout();
            var _container = list_gifs_trending.ContainerFromItem(list_gifs_trending.SelectedItem);
            var _children = allChildren(_container);
            var _control = _children.OfType<Image>().First(x => x.Name == "gif_image");

            GifImageSource _gif = AnimationBehavior.GetGifImageSource(_control);
            if (_gif != null)
            {
                AnimationBehavior.SetRepeatBehavior(_control, new RepeatBehavior(3));
                _gif.Start();
            }    

            _item_playing = _gif;
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

        public List<FrameworkElement> allChildren(DependencyObject parent)
        {
            List<FrameworkElement> controls = new List<FrameworkElement>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement)
                {
                    controls.Add(child as FrameworkElement);
                }
                controls.AddRange(allChildren(child));
            }
            return controls;
        }

        private void list_gifs_trending_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            list_gifs_trending.UpdateLayout();
            var _item = (e.OriginalSource as FrameworkElement).DataContext;
            
            showNotification();
            var dataPackage = new DataPackage();
            string text = "";
            if (App.source.Equals("riffsy"))
            {
                Result result = _item as Result;
                text = result.image_link;
            }
            else if (App.source.Equals("giphy"))
            {
                Datum datum = _item as Datum;
                text = datum.image_link;
            }
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        private void list_gifs_search_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            list_gifs_search.UpdateLayout();
            var _item = (e.OriginalSource as FrameworkElement).DataContext;

            showNotification();
            var dataPackage = new DataPackage();
            string text = "";
            if (App.source.Equals("riffsy"))
            {
                Result result = _item as Result;
                text = result.image_link;
            }
            else if (App.source.Equals("giphy"))
            {
                Datum datum = _item as Datum;
                text = datum.image_link;
            }
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        private async void gifgit_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/sskodje/GifImageSource"));
        }
    }
}
