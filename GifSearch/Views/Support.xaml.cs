using GifSearch.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
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

    public sealed partial class Support : Page
    {
        public Support()
        {
            this.InitializeComponent();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (App.rootFrame.CanGoBack)
                App.rootFrame.GoBack();
        }

        private async void submit_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage _email = new EmailMessage();
            _email.To.Add(new EmailRecipient("gifsearch@fxprodrigues.com"));

            String tmp_type = "";
            switch(type.SelectedIndex)
            {
                case 0:
                    tmp_type = "Suggestion";
                    break;
                case 1:
                    tmp_type = "Bug / Problem";
                    break;
                case 2:
                    tmp_type = "Contact";
                    break;
            }

            _email.Subject = String.Format("{0} - {1}", tmp_type, name.Text);
            _email.Body = String.Format("Message:\n{0}\n\nContact Email:\n{1}\n\n", message.Text, email.Text); ;
            NotificationBarFacade.displayStatusBarMessage("Sending support message...", false);
            await EmailManager.ShowComposeNewEmailAsync(_email);
        }
    }
}
