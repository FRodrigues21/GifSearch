using GifSearch.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GifSearch.Views
{

    public sealed partial class Settings : Page
    {

        public static Boolean code_activated = false;

        public Settings()
        {
            this.InitializeComponent();
            NotificationBarFacade.hideStatusBar();
            fillButtons();
            version.Text = "Application version: " + App.version;
            code_activated = true;
            display_type.SelectedIndex = UserFacade.getDisplayQuality();
            download_type.SelectedIndex = UserFacade.getDownloadQuality();
            code_activated = false;
        }

        private async void fillButtons()
        {
            StorageFolder gif = await UserFacade.getImageFolderPath();
            gifbutton.Content = gif.Name;
            StorageFolder mp4 = await UserFacade.getVideoFolderPath();
            mp4button.Content = mp4.Name;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (App.rootFrame.CanGoBack)
                App.rootFrame.GoBack();
        }

        private void display_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!code_activated)
                UserFacade.setDisplayQuality(((ComboBox)sender).SelectedIndex);
        }

        private void download_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!code_activated)
                UserFacade.setDownloadQuality(((ComboBox)sender).SelectedIndex);
        }

        private async void gifbutton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if(folder != null)
                UserFacade.setImageFolderPath(folder.Path);
            StorageFolder tmp = await UserFacade.getImageFolderPath();
            gifbutton.Content = tmp.Name;
        }

        private async void mp4button_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if(folder != null)
                UserFacade.setVideoFolderPath(folder.Path);
            StorageFolder tmp = await UserFacade.getVideoFolderPath();
            mp4button.Content = tmp.Name;
        }
    }
}
