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

        public async static void setTrendingList(ObservableCollection<Datum> list)
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

        public async static Task<ObservableCollection<Datum>> getTrendingList()
        {
            Debug.WriteLine("ACTIVATED: getTrendingList()\n");
            var data = await readStringFromLocalFile("trending.txt");
            try
            {
                if (data != null)
                {
                    ObservableCollection<Datum> list = JsonConvert.DeserializeObject<ObservableCollection<Datum>>(data);
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

        public static void addFavorite(Object obj)
        {
            ObservableCollection<FavoriteItem> tmp = getFavorites();
            tmp.Add(new FavoriteItem(getSource(), obj));
            setFavorites(tmp);
        }

        public static ObservableCollection<FavoriteItem> getFavorites()
        {
            var data = (string)getValue("favorites");
            if (data != null)
                return JsonConvert.DeserializeObject<ObservableCollection<FavoriteItem>>(data);
            else
                return new ObservableCollection<FavoriteItem>();
        }

        public static void setFavorites(ObservableCollection<FavoriteItem> list)
        {
            var data = JsonConvert.SerializeObject(list);
            setValue("favorites", data);
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
                Debug.WriteLine("Exception: File Writing!");
            }
            return text;
        }

    }
}
