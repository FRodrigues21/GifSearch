using GifSearch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GifSearch.Controllers
{
    class UserFacade
    {

        private static ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;

        public async static Task<StorageFolder> getVideoFolderPath()
        {
            Debug.WriteLine("ACTIVATED: getVideoFolderPath()\n");
            var tmp = getValue("video_folder");
            try
            {
                if (tmp != null)
                    return await StorageFolder.GetFolderFromPathAsync((string)tmp);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting video folder!");
            }
            setVideoFolderPath(KnownFolders.VideosLibrary.Path);
            return KnownFolders.VideosLibrary;
        }

        public static void setVideoFolderPath(string path)
        {
            setValue("video_folder", path);
        }

        public async static Task<StorageFolder> getImageFolderPath()
        {
            Debug.WriteLine("ACTIVATED: getImageFolderPath()\n");
            var tmp = getValue("image_folder");
            try
            {
                if (tmp != null)
                    return await StorageFolder.GetFolderFromPathAsync((string)tmp);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting img folder!");
            }
            setImageFolderPath(KnownFolders.PicturesLibrary.Path);
            return KnownFolders.PicturesLibrary;
        }

        public static void setImageFolderPath(string path)
        {
            setValue("image_folder", path);
        }

        public static int getDownloadQuality()
        {
            Debug.WriteLine("ACTIVATED: getDownloadQuality()\n");
            var tmp = getValue("download");
            try
            {
                if (tmp != null)
                    return (int)tmp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting user download quality!");
            }
            setDownloadQuality(2);
            return 2;
        }

        public static void setDownloadQuality(int quality)
        {
            setValue("download", quality);
        }

        public static int getDisplayQuality()
        {
            Debug.WriteLine("ACTIVATED: getDisplayQuality()\n");
            var tmp = getValue("display");
            try
            {
                if (tmp != null)
                    return (int)tmp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting user display quality!");
            }
            setDisplayQuality(1);
            return 1;
        }

        public static void setDisplayQuality(int quality)
        {
            setValue("display", quality);
        }

        public static string getVersion()
        {
            Debug.WriteLine("ACTIVATED: getVersion()\n");
            var tmp = getValue("version");
            try
            {
                if (tmp != null)
                    return (string)tmp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting user version!");
            }
            setVersion(App.version);
            return null;
        }

        public static void setVersion(string version)
        {
            Debug.WriteLine("ACTIVATED: setVersion()\n");
            setValue("version", version);
        }


        // REVIEW INFO
        public static int getReviewed()
        {
            Debug.WriteLine("ACTIVATED: getReviewed()\n");
            var tmp = getValue("review");
            try
            {
                if (tmp != null)
                    return (int)tmp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting user reviewed!");
            }
            setLogged(0);
            return 0;
        }

        public static void setReviewed(int n)
        {
            Debug.WriteLine("ACTIVATED: setReviewed()\n");
            setValue("review", n);
        }

        // LOGGED INFO
        public static int getLogged()
        {
            Debug.WriteLine("ACTIVATED: getLogged()\n");
            var tmp = getValue("logged");
            try
            {
                if (tmp != null)
                    return (int)tmp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting user limit!");
            }
            setLogged(0);
            return 0;
        }

        public static void setLogged(int n)
        {
            Debug.WriteLine("ACTIVATED: setLogged()\n");
            setValue("logged", n);
        }

        // LAST TRENDING UPDATE
        public static void setLastTrendingUpdate()
        {
            Debug.WriteLine("ACTIVATED: setLastTrendingUpdate()\n");
            try
            {
                var data = JsonConvert.SerializeObject(DateTime.Now);
                setValue("trending_update", data);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Serializing Trending Update Date!");
            }
        }

        public static double getLastTrendingUpdate()
        {
            Debug.WriteLine("ACTIVATED: getLastTrendingUpdate()\n");
            var data = (String)getValue("trending_update");
            try
            {
                DateTime last = JsonConvert.DeserializeObject<DateTime>(data);
                if (last != null)
                    return (DateTime.Now - last).TotalSeconds;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Deserializing Trending Update Date!");
            }
            setLastTrendingUpdate();
            return 0;
        }

        public async static void setTrendingList(ObservableCollection<Result> list)
        {
            Debug.WriteLine("ACTIVATED: setTrendingList()\n");
            try
            {
                var data = JsonConvert.SerializeObject(list);
                await saveStringToLocalFile("trending.txt", data);
                setLastTrendingUpdate();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Serializing Trending List!");
            }
        }

        public async static Task<ObservableCollection<Result>> getTrendingList()
        {
            Debug.WriteLine("ACTIVATED: getTrendingList()\n");
            var data = await readStringFromLocalFile("trending.txt");
            try
            {
                if (data != null)
                {
                    ObservableCollection<Result> list = JsonConvert.DeserializeObject<ObservableCollection<Result>>(data);
                    return list;
                }
                    
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Deserializing Trending List!");
            }
            return null;
        }


        // CONTENT SOURCE

        public static String getSource()
        {
            Debug.WriteLine("ACTIVATED: getSource()\n");
            var tmp = getValue("source");
            if (tmp != null)
                return (String)tmp;
            else
            {
                setSource("giphy");
                return "giphy";
            }
        }

        public static void setSource(String source)
        {
            Debug.WriteLine("ACTIVATED: setSource()\n");
            setValue("limit", source);
        }

        // CONTENT LIMIT

        public static int getLimit()
        {
            Debug.WriteLine("ACTIVATED: getLimit()\n");
            var tmp = getValue("limit");
            try
            {
                if (tmp != null)
                    return (int)tmp;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: Getting user limit!");
            }
            setLimit(20);
            return 20;
        }

        public static void setLimit(int n)
        {
            Debug.WriteLine("ACTIVATED: setLimit()\n");
            if (n > 8)
                setValue("limit", n);
        }

        // USER FAVORITES

        public async static Task<Boolean> hasFavorites()
        {
            ObservableCollection<Result> tmp = await getFavorites();
            if(tmp != null)
                return tmp.Count >= 1;
            return false;
        }

        public async static Task<Boolean> hasFavorite(String id)
        {
            ObservableCollection<Result> tmp = await getFavorites();
            if(tmp != null)
            {
                foreach (Result d in tmp)
                {
                    if (d.id == id)
                        return true;
                }
            }
            return false;
        }

        public async static void removeFavorite(Result obj)
        {
            ObservableCollection<Result> tmp = await getFavorites();
            ObservableCollection<Result> it = new ObservableCollection<Result>(tmp);
            foreach (Result d in it)
                if (d.id == obj.id)
                    tmp.Remove(d);
            setFavorites(tmp);
        }

        public async static void addFavorite(Result obj)
        {
            ObservableCollection<Result> tmp = await getFavorites();
            tmp.Add(obj);
            setFavorites(tmp);
        }

        public async static Task<ObservableCollection<Result>> getFavorites()
        {
            Debug.WriteLine("ACTIVATED: getFavorites()\n");
            var data = await readStringFromLocalFile("favorites.txt");
            try
            {
                if (data != null)
                    return JsonConvert.DeserializeObject<ObservableCollection<Result>>(data);
            }
            catch(Exception e)
            {
                Debug.WriteLine("Exception: Reading favorites list.");
            }
            setFavorites(new ObservableCollection<Result>());
            return null;
        }

        public async static void setFavorites(ObservableCollection<Result> list)
        {
            var data = JsonConvert.SerializeObject(list);
            if (data != null)
               await saveStringToLocalFile("favorites.txt", data);
        }

        // STORAGE OPERATIONS

        public static void setValue(String key, Object obj)
        {
            if (settings.Values.ContainsKey(key))
                settings.Values[key] = obj;
            else
                settings.Values.Add(key, obj);
        }

        public static Object getValue(String key)
        {
            if (settings.Values.ContainsKey(key))
                return settings.Values[key];
            return null;
        }


        public static async Task saveStringToLocalFile(string filename, string content)
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(content.ToCharArray());
            
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        public static async Task<string> readStringFromLocalFile(string filename)
        {
            StorageFolder local = ApplicationData.Current.LocalFolder;
            string text = null;
            try
            {
                Stream stream = await local.OpenStreamForReadAsync(filename);
                using (StreamReader reader = new StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: File Reading!");
                return null;
            }
            return text;
        }

    }
}
