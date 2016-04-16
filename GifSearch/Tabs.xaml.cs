using GifImage;
using GifSearch.Controllers;
using GifSearch.Models;
using GifSearch.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace GifSearch
{
    public sealed partial class Tabs : Page
    {

        private static Boolean code_caused = false;
        private static ResourceLoader res { get; set; }
        private static AppBar _appbar { get; set; }
        private static AppBarButton _favorite { get; set; }
        private static PlayingItem selected_gif { get; set; }
        private static Boolean download_started = false;
        private static String _path { get; set; }

        public Tabs()
        {
            this.InitializeComponent();
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
            _appbar = appbar;
            _favorite = favorite;
            selected_gif = new PlayingItem();
            res = ResourceLoader.GetForCurrentView();
            this.loadChangeLog();
        }

        public static async void playGifAnimation(Object list, Object item)
        {
            if (item != null)
            {
                selected_gif.instance = item;
                if (selected_gif != null)
                    selected_gif.pause();

                Boolean isFavorite = await UserFacade.hasFavorite(((Result)selected_gif.instance).id);
                if (isFavorite)
                    _favorite.Icon = new SymbolIcon(Symbol.UnFavorite);
                else
                    _favorite.Icon = new SymbolIcon(Symbol.Favorite);

                GifImageSource _gif;
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    var _container = ((ListView)list).ContainerFromItem(item);
                    var _children = allChildren(_container);
                    var _control = _children.OfType<Image>().First(x => x.Name == "gif_image");
                    _gif = AnimationBehavior.GetGifImageSource(_control);
                }
                else
                {
                    var _container = ((GridView)list).ContainerFromItem(item);
                    var _children = allChildren(_container);
                    var _control = _children.OfType<Image>().First(x => x.Name == "gif_image");
                    _gif = AnimationBehavior.GetGifImageSource(_control);
                }
                if (_gif != null)
                {
                    selected_gif.sourceInstance = _gif;
                    selected_gif.play();
                }
            }
        }

        public static List<FrameworkElement> allChildren(DependencyObject parent)
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

        private async void copy_Click(object sender, RoutedEventArgs e)
        {
            Result datum = selected_gif.instance as Result;
            if (datum != null)
            {
                var dataPackage = new DataPackage();
                string text = "";
                text = datum.image_link;
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);
                NotificationBarFacade.displayStatusBarMessage(res.GetString("TrendingMessage_CopySuccess"), true);
                await Task.Delay(3000);
                NotificationBarFacade.hideStatusBar();
            }
        }

        private async void favorite_Click(object sender, RoutedEventArgs e)
        {
            var item = selected_gif.instance;
            if (item != null)
            {
                Boolean isFavorite = await UserFacade.hasFavorite(((Result)item).id);
                if (isFavorite)
                {
                    favorite.Icon = new SymbolIcon(Symbol.Favorite);
                    NotificationBarFacade.displayStatusBarMessage(res.GetString("TrendingMessage_FavRem"), true);
                    UserFacade.removeFavorite((Result)selected_gif.instance);
                }
                else
                {
                    favorite.Icon = new SymbolIcon(Symbol.UnFavorite);
                    NotificationBarFacade.displayStatusBarMessage(res.GetString("TrendingMessage_FavAdd"), true);
                    UserFacade.addFavorite((Result)selected_gif.instance);
                }

                await Task.Delay(3000);
                NotificationBarFacade.hideStatusBar();
            }
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (selected_gif.instance != null && !download_started)
            {
                download_started = true;
                NotificationBarFacade.displayStatusBarMessage("Starting media download...", false);
                Result datum = (Result)selected_gif.instance;
                MessageDialog mydial = new MessageDialog(res.GetString("DialogThird_Content"));
                mydial.Title = res.GetString("DialogThird_Title");
                string gif = String.Format(".gif ({0} KB)", await DownloadFacade.getSizeFromSource(datum.image_link));
                string mp4 = String.Format(".mp4 ({0} KB)", await DownloadFacade.getSizeFromSource(datum.image_video));
                mydial.Commands.Add(new UICommand(gif, new UICommandInvokedHandler(downloadMediaGif)));
                mydial.Commands.Add(new UICommand(mp4, new UICommandInvokedHandler(downloadMediaMp4)));
                if (!ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    mydial.Commands.Add(new UICommand(res.GetString("DialogThird_Button3"), new UICommandInvokedHandler(cancelClick)));
                await mydial.ShowAsync();
                download_started = false;
            }
        }

        private void cancelClick(IUICommand command) { }

        private async void downloadMediaGif(IUICommand command)
        {
            Result datum = selected_gif.instance as Result;
            if (datum != null)
            {
                String filename = String.Format("riffsy_{0}.gif", datum.id);
                await DownloadFacade.downloadFromSource(filename, datum.image_link, "image");
            }
        }

        private async void downloadMediaMp4(IUICommand command)
        {
            Result datum = selected_gif.instance as Result;
            if (datum != null)
            {
                String filename = String.Format("riffsy_{0}.mp4", datum.id);
                await DownloadFacade.downloadFromSource(filename, datum.image_video, "video");
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (selected_gif != null)
            {
                if (!selected_gif.state)
                    selected_gif.play();
                else
                    selected_gif.pause();
            }
        }

        private void support_Click(object sender, RoutedEventArgs e)
        {
            App.rootFrame.Navigate(typeof(Support));
        }

        private async void rate_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            App.rootFrame.Navigate(typeof(Settings));
        }

        private async void loadChangeLog()
        {
            string version = UserFacade.getVersion();
            if ((App.user_logged == 1 || version == null || !version.Equals(App.version)) && !App.user_showed)
            {
                UserFacade.setDisplayQuality(1);
                UserFacade.setDownloadQuality(2);
                App.user_showed = true;
                UserFacade.setVersion(App.version);
                string content = String.Format("{0}\n\n- Added Portuguese and Spanish translations (more languages are coming!)\n- You can now share directly into Twitter!\n- Added option to choose default download folders\n\nWarning: The GIF take a while to display on mobile, please be patient :/", App.version);
                MessageDialog mydial = new MessageDialog(content);
                mydial.Title = res.GetString("DialogFirst_Title");
                mydial.Commands.Add(new UICommand(res.GetString("DialogFirst_Button1"), new UICommandInvokedHandler(CommandInvokedHandler_continueclick)));
                mydial.Commands.Add(new UICommand(res.GetString("DialogFirst_Button2"), new UICommandInvokedHandler(CommandInvokedHandler_reviewclick)));
                await mydial.ShowAsync();
            }
            else if(App.user_logged > 1 && App.user_logged % 5 == 0 && UserFacade.getReviewed() == 0 && !App.user_showed)
            {
                App.user_showed = true;
                MessageDialog mydial = new MessageDialog(res.GetString("DialogSecond_Content"));
                mydial.Title = res.GetString("DialogSecond_Title");
                mydial.Commands.Add(new UICommand(res.GetString("DialogFirst_Button2"), new UICommandInvokedHandler(CommandInvokedHandler_reviewclick)));
                mydial.Commands.Add(new UICommand(res.GetString("DialogFirst_Button1"), new UICommandInvokedHandler(CommandInvokedHandler_continueclick)));
                await mydial.ShowAsync();
            }
        }

        private void CommandInvokedHandler_continueclick(IUICommand command) { }

        private async void CommandInvokedHandler_reviewclick(IUICommand command)
        {
            UserFacade.setReviewed(1);
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!code_caused)
            {
                switch(((Pivot)sender).SelectedIndex)
                {
                    case 0:
                        App.pivot_index = 0;
                        frame_1.Navigate(typeof(Trending));
                        break;
                    case 1:
                        App.pivot_index = 1;
                        frame_2.Navigate(typeof(Search));
                        break;
                    case 2:
                        App.pivot_index = 2;
                        frame_3.Navigate(typeof(Favorites));
                        break;
                }
            }
        }

        private async void share_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog mydial = new MessageDialog("Sharing a GIF it's still not support by all Windows 10 apps\n\nTwitter, Messaging (use Normal download quality) and Skype are some examples of apps that support GIF sharing!\nThe sharing it's still buggy, so it may take some time for the GIF to appear in the app you are sharing to.");
            mydial.Title = "gif Search";
            mydial.Commands.Add(new UICommand("Share GIF", new UICommandInvokedHandler(CommandInvokedHandler_shareclick)));
            mydial.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(CommandInvokedHandler_continueclick)));
            await mydial.ShowAsync();
        }

        private void CommandInvokedHandler_shareclick(IUICommand command) {
            DataTransferManager.ShowShareUI();
        }

        private async void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            Result datum = selected_gif.instance as Result;
            args.Request.GetDeferral();

            List<StorageFile> listfiles = new List<StorageFile>();

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage message = await httpClient.GetAsync(datum.image_url);
            String filename = String.Format("riffsy_{0}.gif", datum.id);
            StorageFolder myfolder = await UserFacade.getImageFolderPath();
            StorageFile SampleFile = await myfolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            byte[] file = await message.Content.ReadAsByteArrayAsync();
            await FileIO.WriteBytesAsync(SampleFile, file);
            var files = await myfolder.GetFilesAsync();

            listfiles.Add(SampleFile);

            args.Request.Data.Properties.Title = "GIF " + datum.title + " from riffsy";

            args.Request.Data.SetStorageItems(listfiles);

            args.Request.GetDeferral().Complete();
        }

    }
}
