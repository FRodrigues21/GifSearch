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
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Net.Http;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

namespace GifSearch
{

    public sealed partial class MainPage : Page
    {

        private string current_text = "";
        private Pivot pivot = null;
        private PlayingItem _item_playing;
        private bool page_triggered = false;

        public MainPage()
        {
            this.InitializeComponent();
            reviewfunction();
            pivot = pivot_app;
            _item_playing = new PlayingItem();
            changeLogShow();
            row_notification.Visibility = Visibility.Visible;
        }

        public async void changeLogShow()
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey("use"))
            {
                settings.Values.Add("use", 0);
                string content = String.Format("{0}\n\n- Added .mp4 download option (only Giphy supported)\n- Fixed review app link", App.version);
                MessageDialog mydial = new MessageDialog(content);
                mydial.Title = "What's new in gif Search?";
                mydial.Commands.Add(new UICommand("To the app! Quickly!", new UICommandInvokedHandler(this.CommandInvokedHandler_continueclick)));
                mydial.Commands.Add(new UICommand("Review the app now!", new UICommandInvokedHandler(this.CommandInvokedHandler_yesclick)));
                await mydial.ShowAsync();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("NAVIGATED!");
            page_triggered = true;
            if (App.source == "giphy")
                content_select.SelectedIndex = 0;
            else
                content_select.SelectedIndex = 1;
            page_triggered = false;
        }

        private void CommandInvokedHandler_continueclick(IUICommand command)
        {
            list_gifs_trending_load();
        }

        public async void reviewfunction()
        {
            var settings = ApplicationData.Current.LocalSettings;
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
                    MessageDialog mydial = new MessageDialog("\n\nThank you for using this application.\nWould you like to give some time to rate and review this application to help us improve?");
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

        private void CommandInvokedHandler_noclick(IUICommand command) { }

        private async void CommandInvokedHandler_yesclick(IUICommand command)
        {
            list_gifs_trending_load();
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["rcheck"] = 1;
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
        }

        private async void list_gifs_trending_load()
        {
            appbar.Visibility = Visibility.Collapsed;
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

        private void CommandInvokedHandler_tryagain_trending(IUICommand command) { }

        private void CommandInvokedHandler_tryagain_search(IUICommand command) { }

        private void button_search_Click(object sender, RoutedEventArgs e)
        {
            if (textbox_search.Text != null && !textbox_search.Text.Equals(current_text))
            {
                searchClick();
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

        private async void searchClick()
        {
            list_gifs_search.SelectedIndex = -1;
            list_gifs_search.UpdateLayout();
            if (textbox_search.Text != null)
            {
                appbar.Visibility = Visibility.Collapsed;
                refresh_search.Visibility = Visibility.Collapsed;
                list_gifs_search.Visibility = Visibility.Collapsed;
                progressring_loading.IsActive = true;
                if (App.source.Equals("riffsy"))
                {
                    var collection = (ObservableCollection<Result>)null;
                    try
                    {
                        list_gifs_search.ItemsSource = new ObservableCollection<Result>();
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
                        list_gifs_search.ItemsSource = new ObservableCollection<Result>();
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

        private async void showNotification(string icon, string text, SolidColorBrush color)
        {
            row_notification.Background = color;
            notification_icon.Text = icon;
            notification_text.Text = text;
            SlideIn.Begin();
            await Task.Delay(3000);
            SlideOut.Begin();
        }

        private void pivot_app_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("PIVOT CHANGED!");
            appbar.Visibility = Visibility.Collapsed;
            if (App.changed)
            {
                switch (((Pivot)sender).SelectedIndex)
                {
                    case 0:
                        if (App.changed)
                            list_gifs_trending_load();
                        App.changed = false;
                        list_gifs_search.SelectedIndex = -1;
                        break;

                    case 1:
                        if (App.changed)
                            searchClick();
                        App.changed = false;
                        list_gifs_trending.SelectedIndex = -1;
                        break;
                }
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
            if (e.Key == VirtualKey.Enter)
            {
                loseFocus(sender);
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();
            }
        }

        private void list_gifs_search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            playGifAnimation(list_gifs_search, list_gifs_search.SelectedItem);
        }

        private void list_gifs_trending_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            playGifAnimation(list_gifs_trending, list_gifs_trending.SelectedItem);
        }

        private void playGifAnimation(ListView list, Object item)
        {
            if (item != null)
            {
                _item_playing.instance = item;
                if (_item_playing != null)
                    _item_playing.pause();
                list_gifs_trending.UpdateLayout();
                var _container = list.ContainerFromItem(item);
                var _children = allChildren(_container);
                var _control = _children.OfType<Image>().First(x => x.Name == "gif_image");

                GifImageSource _gif = AnimationBehavior.GetGifImageSource(_control);
                if (_gif != null)
                {
                    _item_playing.sourceInstance = _gif;
                    _item_playing.play();
                }
                appbar.Visibility = Visibility.Visible;
            }
        }

        private void refresh_trending_Click(object sender, RoutedEventArgs e)
        {
            list_gifs_trending_load();
        }

        private void refresh_search_Click(object sender, RoutedEventArgs e)
        {
            searchClick();
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

        private void appbar_copy_Click(object sender, RoutedEventArgs e)
        {
            list_gifs_trending.UpdateLayout();
            var _item = _item_playing;

            showNotification("", "GIF link copied to clipboard!", new SolidColorBrush(Colors.LimeGreen));
            var dataPackage = new DataPackage();
            string text = "";
            if (App.source.Equals("riffsy"))
            {
                Result result = _item_playing.instance as Result;
                text = result.image_link;
            }
            else if (App.source.Equals("giphy"))
            {
                Datum datum = _item_playing.instance as Datum;
                text = datum.image_link;
            }

            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        private async void appbar_save_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog mydial = new MessageDialog("\nMost Windows Phone apps don't support GIF images at the moment\nYou may try to download the GIF as .mp4 in order to share it!\n\nIf you only wan't to store it to view later, select .gif");
            mydial.Title = "Downloading gif...";
            mydial.Commands.Add(new UICommand("Download as .gif", new UICommandInvokedHandler(this.downloadMediaGif)));
            mydial.Commands.Add(new UICommand("Download as .mp4", new UICommandInvokedHandler(this.downloadMediaMp4)));
            await mydial.ShowAsync();
        }

        private void downloadMediaGif(IUICommand command)
        {
            if (App.source == "giphy")
                downloadFromSource("gif");
            else
                showNotification("", "The app can't currently save Riffsy GIF's to storage!", new SolidColorBrush(Colors.Red));
        }

        private void downloadMediaMp4(IUICommand command)
        {
            if(App.source == "giphy")
                downloadFromSource("mp4");
            else
                showNotification("", "The app can't currently save Riffsy GIF's to storage!", new SolidColorBrush(Colors.Red));
        }

        private async void downloadFromSource(string type)
        {
            showNotification("", "Downloading media, wait a moment...", new SolidColorBrush(Colors.LimeGreen));
            string url_image = "";
            string url_video = "";
            string url = "";
            if (App.source.Equals("riffsy"))
            {
                Result result = _item_playing.instance as Result;
                url_image = result.image_link;
                url_video = result.image_video;
            }
            else if (App.source.Equals("giphy"))
            {
                Datum datum = _item_playing.instance as Datum;
                url_image = datum.image_link;
                url_video = datum.image_video;
            }
            if (type == "gif")
                url = url_image;
            else
                url = url_video;
            string FileName = Path.GetFileName(url);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage message = await httpClient.GetAsync(url);
            StorageFolder myfolder = null;
            if (type == "gif")
                 myfolder = KnownFolders.SavedPictures;
            else
                myfolder = KnownFolders.VideosLibrary;
            StorageFile SampleFile = await myfolder.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
            byte[] file = await message.Content.ReadAsByteArrayAsync();
            await FileIO.WriteBytesAsync(SampleFile, file);
            var files = await myfolder.GetFilesAsync();
            showNotification("", "Media downloaded sucessfully!", new SolidColorBrush(Colors.LimeGreen));
        }

        private void appbar_startstop_Click(object sender, RoutedEventArgs e)
        {
            if (_item_playing.state == false)
                _item_playing.play();
            else
                _item_playing.pause();
        }

        private void trending_pub_close_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            trending_pub.Height = 0;
        }

        private void content_select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("COMBOBOX MAIN! > " + page_triggered);
            if (pivot != null && !page_triggered)
            {
                appbar.Visibility = Visibility.Collapsed;
                if (giphy.IsSelected)
                {
                    App.source = "giphy";
                }
                else if (riffsy.IsSelected)
                {
                    App.source = "riffsy";
                }
                if (pivot.SelectedIndex == 0)
                {
                    list_gifs_trending_load();
                }
                else if (pivot.SelectedIndex == 1)
                {
                    searchClick();
                }
            }
        }

        private void button_about_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings_About));
        }

        private void textbox_search_GotFocus(object sender, RoutedEventArgs e)
        {
            appbar.Visibility = Visibility.Collapsed;
        }

        /*private void list_gifs_search_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)list_gifs_search.ItemsPanelRoot;
            panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / 2;
        }*/

    }
}
