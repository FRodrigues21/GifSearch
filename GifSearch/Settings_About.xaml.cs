using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GifSearch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_About : Page
    {
        public Settings_About()
        {
            this.InitializeComponent();
        }

        private void button_back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private void number_picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if(!settings.Values.ContainsKey("number"))
            {
                var number = 10;
                if (picker_8.IsSelected)
                    number = 8;
                else if (picker_10.IsSelected)
                    number = 10;
                else
                    number = 20;
                settings.Values.Add("number", number);
                App.limit = number;
            }
            else
            {
                var number = 10;
                if (picker_8.IsSelected)
                    number = 8;
                else if (picker_10.IsSelected)
                    number = 10;
                else
                    number = 20;
                settings.Values["number"] = number;
                App.limit = number;
            }
        }

        private void provider_picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey("provider"))
            {
                var source = "giphy";
                if (picker_giphy.IsSelected)
                    source = "giphy";
                else if (picker_riffsy.IsSelected)
                    source = "riffsy";
                settings.Values.Add("provider", source);
                App.source = source;
            }
            else
            {
                var source = "giphy";
                if (picker_giphy.IsSelected)
                    source = "giphy";
                else if (picker_riffsy.IsSelected)
                    source = "riffsy";
                settings.Values["provider"] = source;
                App.source = source;
            }
        }
    }
}
