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

        private Boolean navigation_caused = true;

        public Trending()
        {
            this.InitializeComponent();
            this.loadGifList();
            navigation_caused = false;
        }

        private void gif_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            appbar.IsOpen = true;
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
