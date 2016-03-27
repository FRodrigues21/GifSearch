using GifSearch.Controllers;
using GifSearch.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Web.Http;

namespace GifSearch
{
    class GifGiphyFacade
    {

        private static string apikey = "dc6zaTOxFJmzC";

        public static Boolean checkInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }

        public static async Task<ObservableCollection<Datum>> searchGif(string search)
        {
            if (!checkInternet())
            {
                return null;
            }

            HttpClient http = new HttpClient();

            String url = String.Format("http://api.giphy.com/v1/gifs/search?q={1}&api_key={0}&limit={2}", apikey, search, UserFacade.getLimit());
            Uri uri = new Uri(url);
            var response = (HttpResponseMessage)null;
            response = await http.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            RootObject_Giphy data = JsonConvert.DeserializeObject<RootObject_Giphy>(body);
            Debug.WriteLine("Search Gifs downloaded: " + data.data.Count);

            return data.data;
        }

        public static async Task<ObservableCollection<Datum>> getTrending()
        {

            Debug.WriteLine("Last update: " + UserFacade.getLastTrendingUpdate() + "\n");

            ObservableCollection<Datum> tmp = await UserFacade.getTrendingList();
            if (!checkInternet())
            {
                if (tmp != null)
                    return tmp;
                return null;
            }

            Double time = UserFacade.getLastTrendingUpdate();
            if ((time > 0 && time <= 800) && tmp != null)
            {
                Debug.WriteLine("Using cached trending list!");
                return tmp;
            }

            HttpClient http = new HttpClient();

            String url = String.Format("http://api.giphy.com/v1/gifs/trending?api_key={0}&limit={1}", apikey, UserFacade.getLimit());
            Uri uri = new Uri(url);

            var response = (HttpResponseMessage)null;
            response = await http.GetAsync(uri);
            var body = await response.Content.ReadAsStringAsync();

            RootObject_Giphy data = JsonConvert.DeserializeObject<RootObject_Giphy>(body);
            Debug.WriteLine("Trending Gifs downloaded: " + data.data.Count);

            UserFacade.setTrendingList(data.data);

            return data.data;
        }

    }
}
