using GifSearch.Views;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GifSearch
{
    public sealed partial class Tabs : Page
    {

        private static Boolean code_caused = false;

        public Tabs()
        {
            this.InitializeComponent();
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!code_caused)
            {
                switch(((Pivot)sender).SelectedIndex)
                {
                    case 0:
                        frame_1.Navigate(typeof(Trending));
                        break;
                    case 1:
                        frame_2.Navigate(typeof(Search));
                        break;
                }
            }
        }
    }
}
