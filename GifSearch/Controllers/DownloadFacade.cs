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

        public async static Task<Boolean> downloadFromSource(String source, String type)
        {
            NotificationBarFacade.displayStatusBarMessage("Downloading media to storage...", false);
            string FileName = Path.GetFileName(source);
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage message = await httpClient.GetAsync(source);
                StorageFolder myfolder = null;

                if (type == "image")
                    myfolder = KnownFolders.SavedPictures;
                else if (type == "video")
                    myfolder = KnownFolders.VideosLibrary;

                StorageFile SampleFile = await myfolder.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
                byte[] file = await message.Content.ReadAsByteArrayAsync();
                await FileIO.WriteBytesAsync(SampleFile, file);
                var files = await myfolder.GetFilesAsync();
                NotificationBarFacade.displayStatusBarMessage("Media was succesfly downloaded to storage!", true);
                return true;
            }
            catch(Exception e)
            {
                NotificationBarFacade.displayStatusBarMessage("Error ocurred while downloading media! Try again.", true);
                Debug.WriteLine("Exception Message: " + e.Message);
                return false;
            }
        }

    }
}
