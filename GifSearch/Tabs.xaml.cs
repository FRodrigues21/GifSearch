using GifSearch.Controllers;
using GifSearch.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine("Logged: " + App.user_logged);
            if(App.user_logged == 1 && !App.user_showed)
            {
                App.user_showed = true;
                string content = String.Format("{0}\n\n- Added filesize to download message\n- Added Support page\n- Fixed download bugs on mobile\n", App.version);
                MessageDialog mydial = new MessageDialog(content);
                mydial.Title = "What's new in gif Search?";
                mydial.Commands.Add(new UICommand("To the app! Quickly!", new UICommandInvokedHandler(CommandInvokedHandler_continueclick)));
                mydial.Commands.Add(new UICommand("Review the app now!", new UICommandInvokedHandler(CommandInvokedHandler_reviewclick)));
                await mydial.ShowAsync();
            }
            else if(App.user_logged > 1 && App.user_logged % 5 == 0 && UserFacade.getReviewed() == 0 && !App.user_showed)
            {
                App.user_showed = true;
                string content = String.Format("I hope you are enjoying using the app...\n\nWould you like to give some time to rate and review this application to help us improve?");
                MessageDialog mydial = new MessageDialog(content);
                mydial.Title = "Thank you very much!";
                mydial.Commands.Add(new UICommand("Review the app now!", new UICommandInvokedHandler(CommandInvokedHandler_reviewclick)));
                mydial.Commands.Add(new UICommand("To the app! Quickly!", new UICommandInvokedHandler(CommandInvokedHandler_continueclick)));
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
    }
}
