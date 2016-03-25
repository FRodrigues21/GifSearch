using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

namespace GifSearch.Controllers
{
    class NotificationBarFacade
    {

        private static StatusBar _bar = StatusBar.GetForCurrentView();
        private static ApplicationViewTitleBar _title = ApplicationView.GetForCurrentView().TitleBar;

        public static async void displayStatusBarMessage(String message, Boolean last)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                _bar.BackgroundOpacity = 1;
                _bar.ProgressIndicator.Text = message;
                await _bar.ProgressIndicator.ShowAsync();
            }
            else
            {
                if(last)
                {
                    string content = String.Format("\n" + message);
                    MessageDialog mydial = new MessageDialog(content);
                    mydial.Title = "gifSearch";
                    mydial.Commands.Add(new UICommand("Dismiss", new UICommandInvokedHandler(CommandInvokedHandler_noclick)));
                    await mydial.ShowAsync();
                }
            }
        }

        private static void CommandInvokedHandler_noclick(IUICommand command) { }

        public static async void hideStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                await _bar.ProgressIndicator.HideAsync();
        }


    }
}
