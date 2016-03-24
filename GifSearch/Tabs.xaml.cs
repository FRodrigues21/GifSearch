using GifSearch.Controllers;
using GifSearch.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GifSearch
{
    public sealed partial class Tabs : Page
    {

        private static Boolean code_caused = false;

        public Tabs()
        {
            this.InitializeComponent();
            this.loadChangeLog();
        }

        private async void loadChangeLog()
        {
            int count = UserFacade.getLogged();
            if(count < 1000)
            {
                string content = String.Format("{0}\n\n- New and improved UI design!\n- Lots of perfomance improvements\n- Save your favorites GIF's!\n- Added cache to Trending List (loads faster now)\n", App.version);
                MessageDialog mydial = new MessageDialog(content);
                mydial.Title = "What's new in gif Search?";
                mydial.Commands.Add(new UICommand("To the app! Quickly!", new UICommandInvokedHandler(this.CommandInvokedHandler_continueclick)));
                mydial.Commands.Add(new UICommand("Review the app now!", new UICommandInvokedHandler(this.CommandInvokedHandler_reviewclick)));
                await mydial.ShowAsync();
            }
        }

        private void CommandInvokedHandler_continueclick(IUICommand command) { }

        private async void CommandInvokedHandler_reviewclick(IUICommand command)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format("ms-windows-store:REVIEW?PFN={0}", Windows.ApplicationModel.Package.Current.Id.FamilyName)));
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
