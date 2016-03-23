using GifSearch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GifSearch.Controllers
{
    class UserFacade
    {


        private static ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;


        // CONTENT SOURCE

        public static String getSource()
        {
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
            setValue("limit", source);
        }

        // CONTENT LIMIT

        public static int getLimit()
        {
            var tmp = getValue("limit");
            if (tmp != null)
                return (int)tmp;
            else
            {
                setLimit(20);
                return 20;
            }
        }

        public static void setLimit(int n)
        {
            if(n > 8)
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

    }
}
