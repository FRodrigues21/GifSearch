using GifImage;
using GifSearch.Controllers;
using GifSearch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Storage;
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

    public sealed partial class Favorites : Page
    {

        private int loaded_count = 0;
        private PlayingItem selected_gif = null;
        private Boolean navigation_caused = true;

        public Favorites()
        {
            this.InitializeComponent();
            this.loadGifList();
            selected_gif = new PlayingItem();
            navigation_caused = false;
        }

        private void gif_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            appbar.IsOpen = true;
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                playGifAnimation((ListView)sender, ((ListView)sender).SelectedItem);
            else
                playGifAnimation((GridView)sender, ((GridView)sender).SelectedItem);
        }

        private async void playGifAnimation(Object list, Object item)
        {
            if (item != null)
            {
                selected_gif.instance = item;
                if (selected_gif != null)
                    selected_gif.pause();

                Boolean isFavorite = await UserFacade.hasFavorite(((Datum)selected_gif.instance).id);
                if (isFavorite)
                    favorite.Icon = new SymbolIcon(Symbol.UnFavorite);
                else
                    favorite.Icon = new SymbolIcon(Symbol.Favorite);

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

        private async void loadGifList()
        {
            NotificationBarFacade.displayStatusBarMessage("Loading gif list...", false);
            if(App.pivot_index == 2)
            {
                var list = await UserFacade.getFavorites();
                if (list == null)
                {
                    await Task.Delay(8000);
                    loadGifList();
                }
                else
                    gif_list.ItemsSource = list;
            }
        }

        private async void copy_Click(object sender, RoutedEventArgs e)
        {

            NotificationBarFacade.displayStatusBarMessage("Link copied to clipboard, go share it!", true);

            var dataPackage = new DataPackage();
            string text = "";

            Datum datum = selected_gif.instance as Datum;
            text = datum.image_link;
            
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);

            await Task.Delay(3000);
            NotificationBarFacade.hideStatusBar();
        }

        private async void favorite_Click(object sender, RoutedEventArgs e)
        {
            Boolean isFavorite = await UserFacade.hasFavorite(((Datum)selected_gif.instance).id);
            if (isFavorite)
            {
                favorite.Icon = new SymbolIcon(Symbol.Favorite);
                NotificationBarFacade.displayStatusBarMessage("GIF removed from the favorites list!", true);
                UserFacade.removeFavorite((Datum)selected_gif.instance);
            }
            else
            {
                favorite.Icon = new SymbolIcon(Symbol.UnFavorite);
                NotificationBarFacade.displayStatusBarMessage("GIF added to the favorites list!", true);
                UserFacade.addFavorite((Datum)selected_gif.instance);
            }

            await Task.Delay(3000);
            NotificationBarFacade.hideStatusBar();
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog mydial = new MessageDialog("Most Windows Phone apps don't support GIF images at the moment\n\nYou may try to download the GIF as .mp4 in order to share it!\nIf you only wan't to store it to view later, select .gif");
            mydial.Title = "Downloading a gif";
            mydial.Commands.Add(new UICommand("Download as .gif", new UICommandInvokedHandler(this.downloadMediaGif)));
            mydial.Commands.Add(new UICommand("Download as .mp4", new UICommandInvokedHandler(this.downloadMediaMp4)));
            mydial.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler_continueclick)));
            await mydial.ShowAsync();
        }

        private void CommandInvokedHandler_continueclick(IUICommand command) { }

        private async void downloadMediaGif(IUICommand command)
        {
            downloadFromSource("gif");

        }

        private async void downloadMediaMp4(IUICommand command)
        {
            downloadFromSource("mp4");
        }

        private async void downloadFromSource(string type)
        {

            NotificationBarFacade.displayStatusBarMessage("Downloading media to storage...", false);

            string url_image = "";
            string url_video = "";
            string url = "";

            Datum datum = selected_gif.instance as Datum;
            url_image = datum.image_link;
            url_video = datum.image_video;

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

            NotificationBarFacade.displayStatusBarMessage("Media was succesfly downloaded to storage!", true);
            await Task.Delay(2000);
            NotificationBarFacade.hideStatusBar();
        }

        private void gif_list_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)((GridView)sender).ItemsPanelRoot;
            panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / 4;
        }

        private void gif_image_Loaded(object sender, RoutedEventArgs e)
        {
            NotificationBarFacade.hideStatusBar();
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (!selected_gif.state)
                selected_gif.play();
            else
                selected_gif.pause();
        }

    }
}
