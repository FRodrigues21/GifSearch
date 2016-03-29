using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GifSearch.Controllers
{
    class DownloadFacade
    {

        private static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f);
        }


        public async static Task<Double> getSizeFromSource(String source)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage message = await httpClient.GetAsync(source);
                long bytes = long.Parse(message.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
                NotificationBarFacade.hideStatusBar();
                return Math.Round(ConvertBytesToMegabytes(bytes));
            }
            catch(Exception e)
            {
                NotificationBarFacade.hideStatusBar();
                return 0;
            }
        }

        public async static Task<Boolean> downloadFromSource(String filename, String source, String type)
        {
            NotificationBarFacade.displayStatusBarMessage("Downloading media to storage...", false);
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage message = await httpClient.GetAsync(source);
            
                StorageFolder myfolder = null;

                if (type == "image")
                    myfolder = KnownFolders.PicturesLibrary;
                else if (type == "video")
                    myfolder = KnownFolders.VideosLibrary;

                StorageFile SampleFile = await myfolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                byte[] file = await message.Content.ReadAsByteArrayAsync();
                await FileIO.WriteBytesAsync(SampleFile, file);
                var files = await myfolder.GetFilesAsync();
                NotificationBarFacade.displayStatusBarMessage("Media was successfully downloaded to storage!", true);
                await Task.Delay(3000);
                NotificationBarFacade.hideStatusBar();
                return true;
            }
            catch(Exception e)
            {
                NotificationBarFacade.displayStatusBarMessage("Error ocurred while downloading media! Try again.", true);
                await Task.Delay(3000);
                NotificationBarFacade.hideStatusBar();
                Debug.WriteLine("Exception Message: " + e.Message);
                return false;
            }
        }

    }
}
