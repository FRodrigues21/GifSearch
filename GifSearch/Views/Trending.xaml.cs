using GifImage;
using GifSearch.Controllers;
using GifSearch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Trending()
        {
            this.InitializeComponent();
            this.loadGifList();
            selected_gif = new PlayingItem();
            navigation_caused = false;
        }

        private void gif_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            if(App.pivot_index == 0)
            {
                var list = await GifGiphyFacade.getTrending();
                if (list == null)
                {
                    error_presenter.Visibility = Visibility.Visible;
                }
                else
                {
                    gif_list.ItemsSource = list;
                    error_presenter.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void copy_Click(object sender, RoutedEventArgs e)
        {
            Datum datum = selected_gif.instance as Datum;
            if(datum != null)
            {
                var dataPackage = new DataPackage();
                string text = "";
                text = datum.image_link;
                dataPackage.SetText(text);
                Clipboard.SetContent(dataPackage);
                NotificationBarFacade.displayStatusBarMessage("Link copied to clipboard, go share it!", true);
                await Task.Delay(3000);
                NotificationBarFacade.hideStatusBar();
            }
        }

        private async void favorite_Click(object sender, RoutedEventArgs e)
        {
            var item = selected_gif.instance;
            if(item != null)
            {
                Boolean isFavorite = await UserFacade.hasFavorite(((Datum)item).id);
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
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (selected_gif.instance != null)
            {
                MessageDialog mydial = new MessageDialog("Most Windows Phone apps don't support GIF images at the moment\n\nYou may try to download the GIF as .mp4 in order to share it!\nIf you only wan't to store it to view later, select .gif");
                mydial.Title = "Downloading a gif";
                mydial.Commands.Add(new UICommand("Download as .gif", new UICommandInvokedHandler(downloadMediaGif)));
                mydial.Commands.Add(new UICommand("Download as .mp4", new UICommandInvokedHandler(downloadMediaMp4)));
                if (!ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    mydial.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(cancelClick)));
                await mydial.ShowAsync();
            }
        }

        private void cancelClick(IUICommand command) { }

        private async void downloadMediaGif(IUICommand command)
        {
            Datum datum = selected_gif.instance as Datum;
            if (datum != null)
                await DownloadFacade.downloadFromSource(datum.image_link, "image");
        }

        private async void downloadMediaMp4(IUICommand command)
        {
            Datum datum = selected_gif.instance as Datum;
            if (datum != null)
                await DownloadFacade.downloadFromSource(datum.image_video, "video");
        }

        private void gif_list_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = (ItemsWrapGrid)((GridView)sender).ItemsPanelRoot;
            panel.ItemWidth = panel.ItemHeight = e.NewSize.Width / 4;
        }

        private void gif_image_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded_count >= UserFacade.getLimit()/2)
            {
                NotificationBarFacade.hideStatusBar();
                loaded_count = 0;
            }
            else
            {
                loaded_count++;
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if(selected_gif != null)
            {
                if (!selected_gif.state)
                    selected_gif.play();
                else
                    selected_gif.pause();
            }
        }

        private void refresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("Refresh Tapped!");
            loadGifList();
        }

        private void support_Click(object sender, RoutedEventArgs e)
        {
            App.rootFrame.Navigate(typeof(Support));
        }

        private async void rate_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
        }
    }
}
