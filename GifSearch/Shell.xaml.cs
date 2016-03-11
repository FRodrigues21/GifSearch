using GifSearch.Views;
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

    public sealed partial class Shell : Page
    {

        private Boolean navigation_caused = true;

        public Shell()
        {
            this.InitializeComponent();
            frame.Navigate(typeof(Trending));
            nav.SelectedIndex = 0;
            navigation_caused = false;
        }

        private void splitviewPaneState()
        {
            this.splitview.IsPaneOpen = !splitview.IsPaneOpen;
        }

        private void hamburguer_button_Click(object sender, RoutedEventArgs e)
        {
            this.splitviewPaneState();
        }

        private void nav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!navigation_caused)
            {
                this.splitviewPaneState();
                Debug.WriteLine("Shell Nav selection changed.");
            }
        }

        private void settings_button_Click(object sender, RoutedEventArgs e)
        {
            // Clear navigation
            navigation_caused = true;
            nav.SelectedIndex = -1;
            navigation_caused = false;
            // Change page title
            title.Text = "SETTINGS";
            // Navigate to settings
            frame.Navigate(typeof(Settings));
        }
    }
}
