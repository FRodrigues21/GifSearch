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

namespace GifSearch
{

    public sealed partial class Settings_About : Page
    {
        public Settings_About()
        {
            this.InitializeComponent();
            updateSelected();
        }

        private void updateSelected()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey("provider"))
            {
                string provider = (string)settings.Values["provider"];
                if (provider == "giphy")
                    provider_picker.SelectedIndex = 0;
                else
                    provider_picker.SelectedIndex = 1;
            }
            else
            {
                provider_picker.SelectedIndex = 0;
            }
            if(settings.Values.ContainsKey("number"))
            {
                int number = (int)settings.Values["number"];
                if (number == 8)
                    number_picker.SelectedIndex = 0;
                else if (number == 10)
                    number_picker.SelectedIndex = 1;
                else if(number == 20)
                    number_picker.SelectedIndex = 2;
            }
            else
            {
                number_picker.SelectedIndex = 0;
            }
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
                int number = 8;
                if (picker_8.IsSelected)
                    number = 8;
                else if (picker_10.IsSelected)
                    number = 10;
                else if(picker_20.IsSelected)
                    number = 20;
                settings.Values.Add("number", number);
                App.limit = number;
                
            }
            else
            {
                int number = (int)settings.Values["number"];
                if (picker_8.IsSelected)
                    number = 8;
                else if (picker_10.IsSelected)
                    number = 10;
                else if (picker_20.IsSelected)
                    number = 20;
                settings.Values["number"] = number;
                App.limit = number;
            }
            Debug.WriteLine("Limit changed to: " + App.limit);
        }

        private void provider_picker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey("provider"))
            {
                string source = "giphy";
                if (picker_giphy.IsSelected)
                    source = "giphy";
                else if (picker_riffsy.IsSelected)
                    source = "riffsy";
                settings.Values.Add("provider", source);
                App.source = source;
            }
            else
            {
                string source = (string)settings.Values["provider"];
                if (picker_giphy.IsSelected)
                    source = "giphy";
                else if (picker_riffsy.IsSelected)
                    source = "riffsy";
                settings.Values["provider"] = source;
                App.source = source;
            }
            App.changed = true;
        }
    }
}
