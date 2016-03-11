using GifImage;
using GifSearch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
            appbar.IsOpen = true;
            ListView list = sender as ListView;
            playGifAnimation(list, list.SelectedItem);
        }

        private void playGifAnimation(ListView list, Object item)
        {
            if (item != null)
            {
                selected_gif.instance = item;
                if (selected_gif != null)
                    selected_gif.pause();
                list.UpdateLayout();
                var _container = list.ContainerFromItem(item);
                var _children = allChildren(_container);
                var _control = _children.OfType<Image>().First(x => x.Name == "gif_image");

                GifImageSource _gif = AnimationBehavior.GetGifImageSource(_control);
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
            App.status_bar.BackgroundOpacity = 1;
            App.status_bar.ProgressIndicator.Text = "Loading gif list...";
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            App.status_bar.ProgressIndicator.ShowAsync();
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            gif_list.ItemsSource = await GifGiphyFacade.getTrending();
            App.status_bar.ProgressIndicator.ProgressValue = 0;
            App.status_bar.ProgressIndicator.Text = "";
        }


    }
}
